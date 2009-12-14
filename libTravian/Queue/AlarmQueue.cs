using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Net.Mail;
using System.Globalization;
using System.Text.RegularExpressions;

namespace libTravian.Queue
{
    public class AlarmQueue : IQueue
    {
        #region IQueue 成员

        public Travian UpCall
        {
            set;
            get;
        }

        [Json]
        public int VillageID
        {
            set;
            get;
        }

        public bool MarkDeleted
        {
            get;
            private set;
        }

        [Json]
        public bool Paused
        {
            set;
            get;
        }

        public string Title
        {
            get
            {
                if (To != null && To.Length > 0)
                    return To[0] + "...";
                return string.Empty;
            }
        }

        public string Status
        {
            get
            {
                return TotalCount.ToString();
            }
        }

        public int CountDown
        {
            get
            {
                if (!UpCall.TD.Villages.ContainsKey(VillageID))
                {
                    MarkDeleted = true;
                    return 86400;
                }

                int value = 0;
                if (this.resumeTime > DateTime.Now)
                {
                    try
                    {
                        value = Convert.ToInt32((this.resumeTime - DateTime.Now).TotalSeconds);
                    }
                    catch (OverflowException)
                    {
                    }
                }

                return value;
            }
        }

        public void Action()
        {
            if (MinimumDelay > 0)
                return;

            bool beAttacked = false;

            var cv = UpCall.TD.Villages[VillageID];
            foreach (TTInfo tt in cv.Troop.Troops)
            {
                if (tt.TroopType == TTroopType.Incoming && !IsTrustful(tt.Owner))
                {
                    beAttacked = true;
                    ++BeAttackCount;
                    if (LatestIncoming == null || tt.FinishTime < LatestIncoming.FinishTime)
                        LatestIncoming = tt;
                }
            }

            if (beAttacked)
            {
                if (SendMail())
                {
                    TotalCount++;
                    this.MinimumDelay = this.MinimumInterval > 0 ? this.MinimumInterval : 3600 + new Random().Next(30, 300);
                }
                else
                {
                    this.MinimumDelay = new Random().Next(300, 600);
                }
            }
            else
                this.MinimumDelay = this.MinimumInterval > 0 ? this.MinimumInterval : 3600 + new Random().Next(30, 300);

        }

        public int QueueGUID
        {
            get { return 11; }
        }

        #endregion

        #region fields

        #region mail fields
        /// <summary>
        /// mail sender from:gmail only up to now
        /// </summary>
        [Json]
        public string From { set; get; }

        /// <summary>
        /// mail sender pass
        /// </summary>
        [Json]
        public string Password { set; get; }

        /// <summary>
        /// smtp server
        /// </summary>
        [Json]
        public string Host { set; get; }

        /// <summary>
        /// smtp port:gmail=587
        /// </summary>
        [Json]
        public int Port { set; get; }

        /// <summary>
        /// mail receivers
        /// </summary>
        [Json]
        public string[] To { set; get; }

        /// <summary>
        /// mail body. at most 350 for sms
        /// </summary>
        string Body
        {
            get
            {
                string format = "{0},{1},{2}({3}) is under attack. totally {4} waves, the latest wave is at {5} from: {6}";
                var cv = UpCall.TD.Villages[VillageID];
                return string.Format(format,
                    UpCall.TD.Server,
                    UpCall.TD.Username,
                    cv.Name,
                    cv.Coord.ToString(),
                    BeAttackCount,
                    LatestIncoming.FinishTime.ToString(CultureInfo.CurrentCulture),
                    LatestIncoming.Owner);
            }
        }
        #endregion

        [Json]
        public string TrustfulUsers
        {
            set;
            get;
        }

        [Json]
        public int MinimumInterval { get; set; }

        public bool IsValid
        {
            get
            {
                string reg = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
                if (string.IsNullOrEmpty(From) && new Regex(reg).IsMatch(From))
                    return false;

                if (string.IsNullOrEmpty(To.Join(",")))
                    return false;

                if (string.IsNullOrEmpty(Password))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Minimum seconds to wait until the mechant resturns
        /// </summary>
        public int MinimumDelay
        {
            get
            {
                int value = 0;
                if (this.resumeTime > DateTime.Now)
                {
                    try
                    {
                        value = Convert.ToInt32((this.resumeTime - DateTime.Now).TotalSeconds);
                    }
                    catch (OverflowException)
                    {
                    }
                }

                return value;
            }
            set
            {
                this.resumeTime = DateTime.Now.AddSeconds(value);
            }
        }

        int TotalCount = 0;
        DateTime resumeTime = DateTime.Now;

        int BeAttackCount = 0;
        TTInfo LatestIncoming = null;
        #endregion

        #region methods
        bool IsTrustful(string user)
        {
            if (string.IsNullOrEmpty(TrustfulUsers))
                return false;

            return TrustfulUsers.Contains(user);
        }

        bool SendMail()
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(To.Join(","));
            msg.From = new MailAddress(From, UpCall.TD.Server, Encoding.UTF8);
            msg.Subject = string.Format("{0}@{1}", UpCall.TD.Server, UpCall.TD.Username);
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = this.Body;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = false;
            msg.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(From, Password);
            client.Port = this.Port;
            client.Host = this.Host;
            client.EnableSsl = true;
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                return true;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                UpCall.DebugLog(ex);
                return false;
            }
        }
        #endregion
    }
}

