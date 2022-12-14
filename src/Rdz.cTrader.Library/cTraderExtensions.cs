using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System.Globalization;

namespace Rdz.cTrader.Library
{
	public static class cTraderExtensions
	{
		public enum enPriceMode
		{
			Pips = 1,
			Ticks = 2
		}
		public static double GetSize(this Symbol symbol, enPriceMode mode)
		{
			double size = double.NaN;
			switch (mode)
			{
				case enPriceMode.Pips:
					size = symbol.PipSize;
					break;
				case enPriceMode.Ticks:
					size = symbol.TickSize;
					break;
			}
			return size;
		}
		public static double ShiftPrice(this Symbol symbol, double fromPrice, int value, enPriceMode mode = enPriceMode.Pips)
		{
			double size = symbol.GetSize(mode);
			if (!double.IsNaN(size))
			{
				double priceDiff = size * value;
				return fromPrice + priceDiff;
			}
			else
				return double.NaN;
		}

		public static double Distance(this Symbol symbol, double highPrice, double lowPrice, bool AlwaysPositive = false, enPriceMode mode = enPriceMode.Pips)
		{
			double size = symbol.GetSize(mode);
			if (AlwaysPositive)
			{
				return Math.Abs((highPrice - lowPrice) / size);
			}
			else
			{
				return (highPrice - lowPrice) / size;
			}
		}

		public static int ConvertToPips(this Symbol symbol, double input)
		{
			return Convert.ToInt32(Math.Abs(input / symbol.PipSize));
		}
		public static int ConvertToTick(this Symbol symbol, double input)
		{
			return Convert.ToInt32(Math.Abs(input / symbol.TickSize));
		}


		#region Other Functions
		public static double Undigit(this double value, int digit)
		{
			return value * Math.Pow(10, digit);
		}

		public static int StepUp(this int value, int step = 1)
		{
			return value + step;
		}
		public static int StepDown(this int value, int step = 1)
		{
			return value - step;
		}

		public static int Reset(this int value, int reset = 0)
		{
			return reset;
		}

		public static TradeType Reverse(this TradeType value)
		{
			return value == TradeType.Buy ? TradeType.Sell : TradeType.Buy;
		}

		public static double LotToVolume(this Symbol symbol, double LotSize)
		{
			return symbol.NormalizeVolumeInUnits(symbol.QuantityToVolumeInUnits(LotSize));
		}


		public static DateTime ParseDateTime(this string input, string DateFormat)
		{
			DateTime dt;
			if (DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
			{
				return dt;
			}
			return DateTime.MinValue;
		}

		public static double ProjectedProfit(this Position input, double projectedPrice)
		{
			switch (input.TradeType)
			{
				case TradeType.Buy:
					return (projectedPrice - input.EntryPrice) * input.VolumeInUnits;
				case TradeType.Sell:
					return (input.EntryPrice - projectedPrice) * input.VolumeInUnits;
				default:
					return double.NaN;
			}
		}

		public static double ProjectedNetProfit(this Position input, double projectedPrice)
		{
			double pp = input.ProjectedProfit(projectedPrice);
			if (double.IsNaN(pp))
			{
				return double.NaN;
			}
			else
			{
				return (pp - input.Commissions - input.Swap);
			}
		}

		public static string DigitFormat(this int digit)
		{
			string sDigit = string.Empty;
			if (digit > -1 && digit == 0)
			{
				sDigit = "0";
			}
			else if (digit > 0)
			{
				sDigit = string.Concat("0.", new String('0', digit));
			}
			return sDigit;
		}
		public static double CutHalf(this double input, int digits = 5)
		{
			return (!double.IsNaN(input) ? Math.Round(input / 2, digits) : input);
		}

		public static double FindCenterAgainst(this double inputA, double inputB, int digits = 5)
		{
			double interval = inputA.CalculateIntervalAgainst(inputB);
			double intervalMid = interval.CutHalf(digits);
			return (inputA > inputB ? inputA - intervalMid : inputB - intervalMid);
		}
		public static double CalculateIntervalAgainst(this double price1, double price2)
		{
			double gap = price1 - price2;
			return Math.Abs(gap);
		}

		public static string ToString(this double input, int digits, bool withSeparator = true)
		{
			return input.ToString(withSeparator ? "#,##" : "#" + "0." + (new string('#', digits - 1)) + "0", System.Globalization.CultureInfo.InvariantCulture);
		}

		#endregion
	}
}