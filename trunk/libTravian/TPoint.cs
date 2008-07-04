﻿/*
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * The Initial Developer of the Original Code is [MeteorRain <msg7086@gmail.com>].
 * Copyright (C) MeteorRain 2007, 2008. All Rights Reserved.
 * Contributor(s): [MeteorRain].
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace libTravian
{
	[Serializable]
	public struct TPoint
	{
		public static readonly TPoint Empty;
		private int x;
		private int y;
		public TPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public bool IsEmpty
		{
			get
			{
				return ((this.x == 0) && (this.y == 0));
			}
		}
		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}
		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}
		public int Z
		{
			get
			{
				return 801 * (400 - y) + (x + 401);
			}
			set
			{
				x = value % 801 - 401;
				y = 400 - value / 801;
			}
		}

		public int[] getArray
		{
			get
			{
				return new int[] { X, Y };
			}
		}

		public static double operator *(TPoint left, TPoint right)
		{
			return Math.Sqrt((left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y));
		}

		public static bool operator ==(TPoint left, TPoint right)
		{
			return ((left.X == right.X) && (left.Y == right.Y));
		}

		public static bool operator !=(TPoint left, TPoint right)
		{
			return !(left == right);
		}

		public static implicit operator TPoint(int z)
		{
			return new TPoint() { Z = z };
		}

		public override bool Equals(object obj)
		{
			if(!(obj is TPoint))
			{
				return false;
			}
			TPoint point = (TPoint)obj;
			return ((point.X == this.X) && (point.Y == this.Y));
		}

		public override int GetHashCode()
		{
			return (X ^ Y);
		}

		public override string ToString()
		{
			return (this.X.ToString(CultureInfo.CurrentCulture) + "|" + this.Y.ToString(CultureInfo.CurrentCulture));
		}
	}


}