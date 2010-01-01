using System;
using System.Collections.Generic;
using System.Text;

namespace PoproTracker
{
	class PeerInfo
	{
		public enum PeerType
		{
			Start, Leech, Seed
		}
		public string PeerId, IP;
		public int Port;
		public DateTime LastSeen;
		public bool Completed
		{
			get
			{
				return Left == 0;
			}
		}
		public long Left;
		public PeerInfo()
		{
			LastSeen = DateTime.Now;
		}
		public bool TooOld
		{
			get
			{
				return DateTime.Now.Subtract(LastSeen).TotalMinutes > 50;
			}
		}
	}
}
