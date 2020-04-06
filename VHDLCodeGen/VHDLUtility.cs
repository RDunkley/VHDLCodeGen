//********************************************************************************************************************************
// Filename:    VHDLUtility.cs
// Owner:       Richard Dunkley
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2020
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace VHDLCodeGen
{
	/// <summary>
	///   Provides various utillity methods to aid in working with 
	/// </summary>
	public static class VHDLUtility
	{
		#region Methods

		/// <summary>
		///   Checks for overlaps in the address length pairs.
		/// </summary>
		/// <typeparam name="T">Integer type of the address.</typeparam>
		/// <param name="valueLengthPairs"><see cref="Tuple"/> array containing the address and length pairs.</param>
		/// <returns>
		///   Returns a <see cref="Tuple"/> array composed of the two indexes where overlaps took place. The indexes are indexes into <paramref name="valueLengthPairs"/> array
		///   or null if no overlaps were found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="valueLengthPairs"/> is a null reference.</exception>
		public static Tuple<int, int>[] CheckForOverlap<T>(Tuple<T, int>[] valueLengthPairs) where T : struct, IComparable, IComparable<T>, IEquatable<T>, IFormattable, IConvertible
		{
			if (valueLengthPairs == null)
				throw new ArgumentNullException(nameof(valueLengthPairs));

			List<Tuple<int, int>> errors = new List<Tuple<int, int>>();
			for (int i = 0; i < valueLengthPairs.Length; i++)
			{
				for (int j = i + 1; j < valueLengthPairs.Length; j++)
				{
					if (GreaterEqualThan<T>(valueLengthPairs[j].Item1, valueLengthPairs[i].Item1))
					{
						if (LessThan<T>(valueLengthPairs[j].Item1, Add<T>(valueLengthPairs[i].Item1, (T)Convert.ChangeType(valueLengthPairs[i].Item2, typeof(T)))))
							errors.Add(new Tuple<int, int>(i, j));
					}
					else
					{
						if (GreaterThan<T>(Add<T>(valueLengthPairs[j].Item1, (T)Convert.ChangeType(valueLengthPairs[j].Item2, typeof(T))), valueLengthPairs[i].Item1))
							errors.Add(new Tuple<int, int>(i, j));
					}
				}
			}
			if (errors.Count > 0)
				return errors.ToArray();
			return null;
		}

		/// <summary>
		///   Generates the clock synchronization process using the specified signals.
		/// </summary>
		/// <param name="clockName">Name of the clock.</param>
		/// <param name="signals">Array of <see cref="SignalInfo"/> objects to synchronize with the clock.</param>
		/// <param name="resetComparison">
		///   Provides the comparison string for a synchronous reset. This string needs to be the full comparison in the if statement. For example:
		///   "RESET = '1'" or "ENABLE = '0' or RESET = '1'". It should not include the if statement or open/closing parenthesis. If this string is
		///   not provided then the reset case is skipped. If the value is specified then the default values of each signal will be pulled from their
		///   <see cref="SignalInfo.DefaultValue"/> property so all values, must have a specified default value.
		/// </param>
		/// <param name="onlyWithPreClock">
		///   If true then only the <see cref="SignalInfo"/> ojects with <see cref="SignalInfo.AddPreClockSignal"/> set to true will be used. If set
		///   to false then all the signals will be added to the process.
		/// </param>
		/// <returns><see cref="ProcessInfo"/> containing the process information.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="clockName"/> is null or empty.</exception>
		public static ProcessInfo GenerateClockSyncProcess(string clockName, SignalInfo[] signals, string resetComparison = null, bool onlyWithPreClock = true,
			Dictionary<SignalInfo,string> additionalAssignments = null)
		{
			if (string.IsNullOrEmpty(clockName))
				throw new ArgumentNullException(nameof(clockName), "The name of the clock was a null reference (or empty).");
			if (signals == null)
				return null;

			List<SignalInfo> sigList = new List<SignalInfo>(signals.Length);
			if(!onlyWithPreClock)
			{
				sigList.AddRange(signals);
			}
			else
			{
				foreach(SignalInfo sig in signals)
				{
					if (sig.AddPreClockSignal)
						sigList.Add(sig);
				}
			}

			if (sigList.Count == 0)
				return null;

			ProcessInfo proc = new ProcessInfo("CLK_SYNC", string.Format("Synchronizes the changing registers with the transition of the {0} clock.", clockName));
			proc.SensitivityList.Add(clockName);
			proc.CodeLines.Add(string.Format("if(rising_edge({0})) then", clockName));
			string pre = string.Empty;
			if (resetComparison != null)
			{
				proc.CodeLines.Add(string.Format("	if({0}) then", resetComparison));
				foreach (SignalInfo sig in sigList)
					proc.CodeLines.Add(string.Format("		{0} <= {1};", sig.Name, sig.DefaultValue));
				foreach(SignalInfo sig in additionalAssignments.Keys)
					proc.CodeLines.Add(string.Format("		{0} <= {1};", sig.Name, sig.DefaultValue));
				proc.CodeLines.Add("	else");
				pre = "	";
			}
			foreach (SignalInfo sig in sigList)
				proc.CodeLines.Add(string.Format("{0}	{1} <= next_{1};", pre, sig.Name));
			foreach(SignalInfo sig in additionalAssignments.Keys)
				proc.CodeLines.Add(string.Format("{0}	{1} <= {2};", pre, sig.Name, additionalAssignments[sig]));
			if (resetComparison != null)
				proc.CodeLines.Add("	end if;");
			proc.CodeLines.Add("end if;");
			return proc;
		}

		/// <summary>
		///   Gets the bit mask value based on the specified start and end bit indexes.
		/// </summary>
		/// <param name="startBit">Starting index of the bit mask.</param>
		/// <param name="endBit">Ending index of the bit mask.</param>
		/// <returns>Bit mask value (value with all bits between and including start and end asserted).</returns>
		/// <exception cref="ArgumentException"><paramref name="startBit"/> or <paramref name="endBit"/> is less than 0 or greater than 63.</exception>
		public static ulong GetBitMask(int startBit, int endBit)
		{
			if (startBit < 0 || startBit > 63)
				throw new ArgumentException(string.Format("The start bit specified ({0}) is less than 0 or greater than 63.", startBit), nameof(startBit));
			if (endBit < 0 || endBit > 63)
				throw new ArgumentException(string.Format("The end bit specified ({0}) is less than 0 or greater than 63.", endBit), nameof(endBit));

			int len;
			if (startBit > endBit)
			{
				len = startBit - endBit + 1;
				return GetBitMask(len) << endBit;
			}
			else
			{
				len = endBit - startBit + 1;
				return GetBitMask(len) << startBit;
			}
		}

		/// <summary>
		///   Gets the bit mask value based on the specified width.
		/// </summary>
		/// <param name="width">Width of the bit mask (in bits).</param>
		/// <returns>Bit mask value.</returns>
		/// <exception cref="ArgumentException"><paramref name="width"/> is less than 1 or greater than 64.</exception>
		public static ulong GetBitMask(int width)
		{
			if (width < 1 || width > 64)
				throw new ArgumentException(string.Format("The width specified ({0}) is less than 1 or greater than 64.", width), nameof(width));

			ulong returnValue = 1;
			for (int i = 1; i < width; i++)
				returnValue = (returnValue << 1) | 1;
			return returnValue;
		}

		/// <summary>
		///   Gets the bit string associated with the value.
		/// </summary>
		/// <param name="value">Value to be represented.</param>
		/// <param name="numberOfBits">Number of bits in the resulting string. If less than needed for the value then the value will be truncated.</param>
		/// <param name="singleString">True if the string should be kept as a single value (not simplified), false if multiple components can be concatenated (Ex: '0' & x"00").</param>
		/// <param name="lowerBitsToDrop">Drops the specified number of bits.</param>
		/// <returns>Bit string.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///   <paramref name="numberOfBits"/> is less than 1 or greater than 64, <paramref name="lowerBitsToDrop"/> is less than 0 or greater than <paramref name="numberOfBits"/>,
		///   or <paramref name="value"/> is larger than 1 for a single bit.
		/// </exception>
		public static string GetBitString(ulong value, int numberOfBits, bool singleString = false, int lowerBitsToDrop = 0)
		{
			if (numberOfBits < 1 || numberOfBits > 64)
				throw new ArgumentOutOfRangeException(nameof(numberOfBits), string.Format("The number of bits specified ({0}) was less than 1, or greater than 64.", numberOfBits));
			if (lowerBitsToDrop < 0 || lowerBitsToDrop >= numberOfBits)
				throw new ArgumentOutOfRangeException(nameof(lowerBitsToDrop), $"The number of bits-to-drop specified ({lowerBitsToDrop}) was less than 0, or greater than or equal to the number of bits ({numberOfBits}).");

			if (numberOfBits == 1)
			{
				if (value > 1)
					throw new ArgumentException(string.Format("The value specified ({0}) is larger than 1 for a single bit (single bit must be 0 or 1).", value), nameof(value));
				if (value == 1)
					return "'1'";
				return "'0'";
			}

			// drop the lower bits by shifting.
			value >>= lowerBitsToDrop;
			numberOfBits -= lowerBitsToDrop;

			int numHexChars = numberOfBits / 4;
			int remBits = numberOfBits % 4;

			string hexString = value.ToString($"X{numHexChars}").Substring(0, numHexChars);
			if (remBits == 0)
				return $"x\"{hexString}\"";

			if (singleString)
				return GetBitStringWithOnlySingleBits(value, 0, numberOfBits - 1);

			string remString = GetBitStringWithOnlySingleBits(value, numHexChars * 4, numberOfBits - 1);
			if (string.IsNullOrEmpty(hexString))
				return $"\"{remString}\"";
			return $"\"{remString}\" & x\"{hexString}\"";
		}

		/// <summary>
		///   Gets the bit string associated with the value, start index, and end index in binary form.
		/// </summary>
		/// <param name="value">Value to be rendered in binary form.</param>
		/// <param name="startBit">Starting bit index.</param>
		/// <param name="endBit">Ending bit index.</param>
		/// <returns>Bit string representing the value specified.</returns>
		/// <exception cref="ArgumentException"><paramref name="startBit"/> or <paramref name="endBit"/> is less than 0 or greater than 63.</exception>
		public static string GetBitStringWithOnlySingleBits(ulong value, int startBit, int endBit)
		{
			if (startBit < 0 || startBit > 63)
				throw new ArgumentException(string.Format("The start bit specified ({0}) is less than 0 or greater than 63.", startBit), nameof(startBit));
			if (endBit < 0 || endBit > 63)
				throw new ArgumentException(string.Format("The end bit specified ({0}) is less than 0 or greater than 63.", endBit), nameof(endBit));

			int start = startBit;
			int len = endBit - startBit + 1;
			if (startBit > endBit)
			{
				start = endBit;
				len = startBit - endBit;
			}
			value = value >> start;

			// Add all the bits to a bitstring.
			char[] bitString = new char[len];
			ulong mask = 1;
			for (int i = 0; i < len; i++)
			{
				if ((value & mask) == mask)
					bitString[len - 1 - i] = '1';
				else
					bitString[len - 1 - i] = '0';
				mask <<= 1;
			}
			return $"\"{new string(bitString)}\"";
		}

		/// <summary>
		///   Gets the byte lane mapping of the specified bit value within a register specified by the width.
		/// </summary>
		/// <param name="startBit">Index of the starting bit in the register width.</param>
		/// <param name="endBit">Index of the ending bit in the register width.</param>
		/// <param name="width">Width of the register containing the start and end bits.</param>
		/// <returns>
		///   Read-only dictionary indexed by the byte-lane and containing a Tuple value that has the start and end bit
		///   index in that byte-lane that will contain data.
		/// </returns>
		/// <remarks>
		///   The returning tuple values can be normalized by subtracting the start bit. If the calling code is attempting
		///   to copy one register to another for byte lane 0. Using the mapping you could do the following:
		///   regA(lookup[0].Item2-1 downto lookup[0].Item1) = regB(lookup[0].Item2-startBit-1 downto lookup[0].Item1-startBit);
		/// </remarks>
		/// <exception cref="ArgumentException">
		///   <paramref name="width"/> is less than one, <paramref name="startBit"/> is less than 0 or greater than or equal to
		///   <paramref name="width"/>, or <paramref name="endBit"/> is less than 0 or greater than or equal to <paramref name="width"/>.
		/// </exception>
		public static ReadOnlyDictionary<int, Tuple<int, int>> GetByteLaneMapping(int startBit, int endBit, int width)
		{
			if (width < 1)
				throw new ArgumentException($"The width specified ({width}) is less than 1.", nameof(width));
			if (startBit < 0 || startBit >= width)
				throw new ArgumentException($"The start bit index specified ({startBit}) is not contained in the register (width: {width}).", nameof(startBit));
			if (endBit < 0 || endBit >= width)
				throw new ArgumentException($"The end bit index specified ({endBit}) is not contained in the register (width: {width}).", nameof(endBit));

			// Orient the start and end bit correctly.
			if (startBit > endBit)
			{
				int temp = endBit;
				endBit = startBit;
				startBit = temp;
			}

			Dictionary<int, Tuple<int, int>> lookup = new Dictionary<int, Tuple<int, int>>();
			int startLane = startBit / 8;
			int endLane = endBit / 8;
			for (int i = startLane; i <= endLane; i++)
			{
				int laneStart = i * 8;
				int laneEnd = i * 8 + 7;
				if (laneStart >= startBit)
				{
					if (laneEnd <= endBit)
					{
						// Entire lane.
						lookup.Add(i, new Tuple<int, int>(laneStart, laneEnd));
					}
					else
					{
						// Starts at lane boundary but ends at endBit.
						lookup.Add(i, new Tuple<int, int>(laneStart, endBit));
					}
				}
				else
				{
					if (laneEnd <= endBit)
					{
						// Starts at startBit and ends at laneEnd.
						lookup.Add(i, new Tuple<int, int>(startBit, laneEnd));
					}
					else
					{
						// Starts at startBit and ends at endBit.
						lookup.Add(i, new Tuple<int, int>(startBit, endBit));
					}
				}
			}
			return new ReadOnlyDictionary<int, Tuple<int, int>>(lookup);
		}

		/// <summary>
		///   Gets the left and right values from a std_logic_vector string.
		/// </summary>
		/// <param name="vector">Standard logic vector string to parse the left and right values from.</param>
		/// <returns><see cref="Tuple"/> containing the start and end values.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="vector"/> is null or whitespace.</exception>
		/// <exception cref="ArgumentException"><paramref name="vector"/> is not in the proper format.</exception>
		public static Tuple<string, string> GetLeftRightFromVector(string vector)
		{
			if (string.IsNullOrWhiteSpace(vector))
				throw new ArgumentNullException(nameof(vector));

			Regex reg = new Regex(@"^std_logic_vector\((<left>.+)\s(downto|to)\s(<right>.+)\)$");
			Match match = reg.Match(vector);
			if (!match.Success)
				throw new ArgumentException("Unable to parse the vector, vector must be in the format 'std_logic_vector(<left> to|downto <right>).", nameof(vector));
			return new Tuple<string, string>(match.Groups["left"].Value, match.Groups["right"].Value);
		}

		/// <summary>
		///   Gets the next power of two that the specified value would be contained in.
		/// </summary>
		/// <param name="value">Value to find the next power of two of.</param>
		/// <returns>Next power of two from the value. For example, if 12 is specified than 4 will be returned because 2^4 = 16 is the next greater value.</returns>
		public static int GetNextPowerOfTwoBitWidth(ulong value)
		{
			ulong mask = 1;
			for (int i = 0; i < 64; i++)
			{
				if (mask > value)
					return i;
				mask <<= 1;
			}
			return 64;
		}

		/// <summary>
		///   Returns the number of address bits needed to contain the address/length pairs specified.
		/// </summary>
		/// <param name="addressLenPairs">Array of address/length pairs that would be contained in the address space.</param>
		/// <returns>Number of address bits needed to contain the address/length pairs.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="addressLenPairs"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="addressLenPairs"/> is an empty array.</exception>
		public static int GetNumberOfAddressBits(Tuple<ulong, int>[] addressLenPairs)
		{
			if (addressLenPairs == null)
				throw new ArgumentNullException(nameof(addressLenPairs));
			if (addressLenPairs.Length == 0)
				throw new ArgumentException($"{nameof(addressLenPairs)} is an empty array.");

			ulong maxSize = 0;
			foreach (Tuple<ulong, int> pair in addressLenPairs)
			{
				if ((pair.Item1 + (ulong)pair.Item2) > maxSize)
					maxSize = pair.Item1 + (ulong)pair.Item2;
			}

			// Get the next power of two that will cover the maximum size.
			return GetNextPowerOfTwoBitWidth(maxSize - 1);
		}

		/// <summary>
		///   Gets the value based on the bits between the start and end bits.
		/// </summary>
		/// <param name="value">Value to extract the sub-value from.</param>
		/// <param name="startBit">Starting bit of the sub-value.</param>
		/// <param name="endBit">Ending bit of the sub-value.</param>
		/// <returns>Sub-value extracted from <paramref name="value"/> based on the start and end indexes.</returns>
		/// <remarks><paramref name="startBit"/> and <paramref name="endBit"/> can be switch to support 'downto' or 'to' orders.</remarks>
		/// <exception cref="ArgumentException"><paramref name="startBit"/> or <paramref name="endBit"/> are less than 0 or greater tahn 63.</exception>
		public static ulong GetSubValue(ulong value, int startBit, int endBit)
		{
			if (startBit < 0 || startBit > 63)
				throw new ArgumentException(string.Format("The start bit specified ({0}) is less than 0 or greater than 63.", startBit), nameof(startBit));
			if (endBit < 0 || endBit > 63)
				throw new ArgumentException(string.Format("The end bit specified ({0}) is less than 0 or greater than 63.", endBit), nameof(endBit));

			ulong returnValue = value & GetBitMask(startBit, endBit);
			if (startBit > endBit)
				return returnValue >> endBit;
			return returnValue >> startBit;
		}

		/// <summary>
		///   Gets the index into the std logic array.
		/// </summary>
		/// <param name="startBit">
		///   Starting index in the vector. If <paramref name="startBit"/> is larger than <paramref name="endBit"/>
		///   then the resulting index will contain a 'downto' (Ex: '(8 downto 5)'). If <paramref name="startBit"/> is less
		///   than <paramref name="endBit"/> then the resulting index will contain a 'to' (Ex: '(5 to 8)').
		/// </param>
		/// <param name="endBit">Ending index in the vector.</param>
		/// <param name="downtoOnly">
		///   Only return 'downto', instead of both 'downto' and 'to'. This means if <paramref name="startBit"/> is less
		///   than <paramref name="endBit"/> then the start and end values will be switched to keep the 'downto' valid.
		/// </param>
		/// <param name="normalize">
		///   Normalize the two numbers so the indexing goes to '0'. This will generate a normalized index around zero.
		///   (Ex: if <paramref name="startBit"/> is 8 and <paramref name="endBit"/> is 5 then the resulting index will be
		///   '(3 downto 0)'.)
		/// </param>
		/// <param name="singleBitIndex">
		///   If <paramref name="startBit"/> is equal to <paramref name="endBit"/> and this value is true then the returned index
		///   will be the single bit (Ex: '(5)'). If false then it will return the 'downto' or 'to' indexing. If 
		///   <paramref name="normalize"/> is true and this value is true then it will return '(0)'.
		/// </param>
		/// <returns>A string containing the index into the array (Ex: '(0 to 15)'). Including parenthesis.</returns>
		/// <exception cref="ArgumentException"><paramref name="startBit"/> or <paramref name="endBit"/> is less than 0 or greater than 63.</exception>
		public static string GetStdLogicArrayIndex(int startBit, int endBit, bool downtoOnly = false, bool normalize = true, bool singleBitIndex = true)
		{
			if (startBit < 0 || startBit > 63)
				throw new ArgumentException(string.Format("The start bit specified ({0}) is less than 0 or greater than 63.", startBit), nameof(startBit));
			if (endBit < 0 || endBit > 63)
				throw new ArgumentException(string.Format("The end bit specified ({0}) is less than 0 or greater than 63.", endBit), nameof(endBit));

			if (startBit == endBit)
			{
				if (singleBitIndex)
				{
					if (normalize)
						return "(0)";
					else
						return string.Format("({0})", startBit);
				}
				else
				{
					if (downtoOnly)
					{
						if (normalize)
							return string.Format("(0 downto 0)");
						else
							return string.Format("({0} downto {0})", startBit);
					}
					else
					{
						if(normalize)
							return string.Format("(0 to 0)");
						else
							return string.Format("({0} to {0})", startBit);
					}
				}
			}

			if (startBit > endBit)
			{
				if (normalize)
					return string.Format("({0} downto 0)", startBit - endBit);
				else
					return string.Format("({0} downto {1})", startBit, endBit);
			}
			else
			{
				if (downtoOnly)
				{
					if (normalize)
						return string.Format("({0} downto 0)", endBit - startBit);
					else
						return string.Format("({0} downto {1})", endBit, startBit);
				}
				else
				{
					if (normalize)
						return string.Format("(0 to {0})", endBit - startBit);
					else
						return string.Format("({0} to {1})", startBit, endBit);
				}
			}
		}

		/// <summary>
		///   Gets the standard logic type based on the start and end bit indexes.
		/// </summary>
		/// <param name="startBit">Starting bit index.</param>
		/// <param name="endBit">Ending bit index.</param>
		/// <param name="downtoOnly">
		///   Only return 'downto', instead of both 'downto' and 'to'. This means if <paramref name="startBit"/> is less
		///   than <paramref name="endBit"/> then the start and end values will be switched to keep the 'downto' valid.
		/// </param>
		/// <returns>
		///   'std_logic' if <paramref name="startBit"/> is equal to <paramref name="endBit"/>, othewise the 'std_logic_vector' with the specified indexes.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="startBit"/> or <paramref name="endBit"/> is less than 0 or greater than 63.</exception>
		public static string GetStdLogicType(int startBit, int endBit, bool downtoOnly = false)
		{
			if (startBit < 0 || startBit > 63)
				throw new ArgumentException(string.Format("The start bit specified ({0}) is less than 0 or greater than 63.", startBit), nameof(startBit));
			if (endBit < 0 || endBit > 63)
				throw new ArgumentException(string.Format("The end bit specified ({0}) is less than 0 or greater than 63.", endBit), nameof(endBit));

			if (startBit == endBit)
				return "std_logic";
			return string.Format("std_logic_vector{0}", GetStdLogicArrayIndex(startBit, endBit, downtoOnly, true, false));
		}

		/// <summary>
		///   Gets the unsigned type based on the start and end bit indexes.
		/// </summary>
		/// <param name="startBit">Starting bit index.</param>
		/// <param name="endBit">Ending bit index.</param>
		/// <param name="downtoOnly">
		///   Only return 'downto', instead of both 'downto' and 'to'. This means if <paramref name="startBit"/> is less
		///   than <paramref name="endBit"/> then the start and end values will be switched to keep the 'downto' valid.
		/// </param>
		/// <returns>
		///   'bit' if <paramref name="startBit"/> is equal to <paramref name="endBit"/>, othewise the 'unsigned' with the specified indexes.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="startBit"/> or <paramref name="endBit"/> is less than 0 or greater than 63.</exception>
		public static string GetUnsignedType(int startBit, int endBit, bool downtoOnly = false)
		{
			if (startBit < 0 || startBit > 63)
				throw new ArgumentException(string.Format("The start bit specified ({0}) is less than 0 or greater than 63.", startBit), nameof(startBit));
			if (endBit < 0 || endBit > 63)
				throw new ArgumentException(string.Format("The end bit specified ({0}) is less than 0 or greater than 63.", endBit), nameof(endBit));

			if (startBit == endBit)
				return "bit";
			return string.Format("unsigned{0}", GetStdLogicArrayIndex(startBit, endBit, downtoOnly, true, false));
		}

		/// <summary>
		///   This method takes in a list of address/length pairs corresponding to addresses and lengths in a memory map. It then
		///   returns a list of address/lengths that represent the simplified used blocks in the memory space.
		/// </summary>
		/// <param name="addressLengthPairs">Array of <see cref="Tuple"/>s containing the address and length of the section.</param>
		/// <returns>
		///   Simplified list of the used blocks in the memory space. For example, if [0,4] and [4,4] are passed in, then the return
		///   list will be a single item of [0,8].
		/// </returns>
		/// <remarks>
		///   This method will ignore overlapping address/length pairs.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="addressLengthPairs"/> is a null reference.</exception>
		public static Tuple<ulong, int>[] SimplifyMemoryMap(Tuple<ulong, int>[] addressLengthPairs)
		{
			if (addressLengthPairs == null)
				throw new ArgumentNullException(nameof(addressLengthPairs), "The address/length pair array is null.");
			if (addressLengthPairs.Length == 0)
				return new Tuple<ulong, int>[0];

			Dictionary<ulong, int> map = new Dictionary<ulong, int>();

			// Order the pairs.
			foreach (Tuple<ulong, int> pair in addressLengthPairs)
			{
				if(map.ContainsKey(pair.Item1))
				{
					// Duplicate address so choose the longer length.
					if (map[pair.Item1] < pair.Item2)
						map[pair.Item1] = pair.Item2;
				}
				else
				{
					// No pair at that address so add it.
					map.Add(pair.Item1, pair.Item2);
				}
			}

			// Combine blocks that are next to each other.
			List<Tuple<ulong, int>> returnBlocks = new List<Tuple<ulong, int>>();
			List<ulong> addresses = new List<ulong>(map.Keys);
			addresses.Sort();
			ulong start = addresses[0];
			int length = map[start];
			for(int i = 1; i < addresses.Count; i++)
			{
				int nextBlockLength = map[addresses[i]];
				if (start + (ulong)length >= addresses[i])
				{
					// The next pair is in this block or directly adjacent to it.
					if ((addresses[i] + (ulong)nextBlockLength) > (start + (ulong)length))
					{
						// Increase the current block length to include all of the next pair.
						length = (int)(addresses[i] - start + (ulong)nextBlockLength);
					}
				}
				else
				{
					// The next pair is not in or directly adjacent to this one so store the current block
					// and make the next pair the current block.
					returnBlocks.Add(new Tuple<ulong, int>(start, length));
					start = addresses[i];
					length = nextBlockLength;
				}
			}

			// Add the last block.
			returnBlocks.Add(new Tuple<ulong, int>(start, length));
			return returnBlocks.ToArray();
		}

		#endregion

		#region Helper Methods

		/// <summary>
		///   Facilitates addition of two integral values.
		/// </summary>
		/// <typeparam name="T">Type of the integers to add.</typeparam>
		/// <param name="item1">First item to be added.</param>
		/// <param name="item2">Second item to be added.</param>
		/// <returns>Result of <paramref name="item1"/> added to <paramref name="item2"/>.</returns>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an integral type.</exception>
		private static T Add<T>(T item1, T item2) where T : struct, IComparable, IComparable<T>, IEquatable<T>, IFormattable, IConvertible
		{
			if (typeof(T) == typeof(byte))
				return (T)Convert.ChangeType(Convert.ToByte(item1) + Convert.ToByte(item2), typeof(T));
			if (typeof(T) == typeof(sbyte))
				return (T)Convert.ChangeType(Convert.ToSByte(item1) + Convert.ToSByte(item2), typeof(T));
			if (typeof(T) == typeof(ushort))
				return (T)Convert.ChangeType(Convert.ToUInt16(item1) + Convert.ToUInt16(item2), typeof(T));
			if (typeof(T) == typeof(short))
				return (T)Convert.ChangeType(Convert.ToInt16(item1) + Convert.ToInt16(item2), typeof(T));
			if (typeof(T) == typeof(uint))
				return (T)Convert.ChangeType(Convert.ToUInt32(item1) + Convert.ToUInt32(item2), typeof(T));
			if (typeof(T) == typeof(int))
				return (T)Convert.ChangeType(Convert.ToInt32(item1) + Convert.ToInt32(item2), typeof(T));
			if (typeof(T) == typeof(ulong))
				return (T)Convert.ChangeType(Convert.ToUInt64(item1) + Convert.ToUInt64(item2), typeof(T));
			if (typeof(T) == typeof(long))
				return (T)Convert.ChangeType(Convert.ToInt64(item1) + Convert.ToInt64(item2), typeof(T));
			throw new ArgumentException("The type of T is not an integral type (byte, sbyte, ushort, short, uint, int, ulong, or long).");
		}

		/// <summary>
		///   Facilitates parameterized comparison of two integral values.
		/// </summary>
		/// <typeparam name="T">Type of the integer to compare.</typeparam>
		/// <param name="item1">First item in the comparison.</param>
		/// <param name="item2">Second item in the comparison.</param>
		/// <returns>True if <paramref name="item1"/> is greater than or equal to <paramref name="item2"/>, false otherwise.</returns>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an integral type.</exception>
		private static bool GreaterEqualThan<T>(T item1, T item2) where T : struct, IComparable, IComparable<T>, IEquatable<T>, IFormattable, IConvertible
		{
			if (typeof(T) == typeof(byte))
				return Convert.ToByte(item1) >= Convert.ToByte(item2);
			if (typeof(T) == typeof(sbyte))
				return Convert.ToSByte(item1) >= Convert.ToSByte(item2);
			if (typeof(T) == typeof(ushort))
				return Convert.ToUInt16(item1) >= Convert.ToUInt16(item2);
			if (typeof(T) == typeof(short))
				return Convert.ToInt16(item1) >= Convert.ToInt16(item2);
			if (typeof(T) == typeof(uint))
				return Convert.ToUInt32(item1) >= Convert.ToUInt32(item2);
			if (typeof(T) == typeof(int))
				return Convert.ToInt32(item1) >= Convert.ToInt32(item2);
			if (typeof(T) == typeof(ulong))
				return Convert.ToUInt64(item1) >= Convert.ToUInt64(item2);
			if (typeof(T) == typeof(long))
				return Convert.ToInt64(item1) >= Convert.ToInt64(item2);
			throw new ArgumentException("The type of T is not an integral type (byte, sbyte, ushort, short, uint, int, ulong, or long).");
		}

		/// <summary>
		///   Facilitates parameterized comparison of two integral values.
		/// </summary>
		/// <typeparam name="T">Type of the integer to compare.</typeparam>
		/// <param name="item1">First item in the comparison.</param>
		/// <param name="item2">Second item in the comparison.</param>
		/// <returns>True if <paramref name="item1"/> is greater than <paramref name="item2"/>, false otherwise.</returns>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an integral type.</exception>
		private static bool GreaterThan<T>(T item1, T item2) where T : struct, IComparable, IComparable<T>, IEquatable<T>, IFormattable, IConvertible
		{
			if (typeof(T) == typeof(byte))
				return Convert.ToByte(item1) > Convert.ToByte(item2);
			if (typeof(T) == typeof(sbyte))
				return Convert.ToSByte(item1) > Convert.ToSByte(item2);
			if (typeof(T) == typeof(ushort))
				return Convert.ToUInt16(item1) > Convert.ToUInt16(item2);
			if (typeof(T) == typeof(short))
				return Convert.ToInt16(item1) > Convert.ToInt16(item2);
			if (typeof(T) == typeof(uint))
				return Convert.ToUInt32(item1) > Convert.ToUInt32(item2);
			if (typeof(T) == typeof(int))
				return Convert.ToInt32(item1) > Convert.ToInt32(item2);
			if (typeof(T) == typeof(ulong))
				return Convert.ToUInt64(item1) > Convert.ToUInt64(item2);
			if (typeof(T) == typeof(long))
				return Convert.ToInt64(item1) > Convert.ToInt64(item2);
			throw new ArgumentException("The type of T is not an integral type (byte, sbyte, ushort, short, uint, int, ulong, or long).");
		}

		/// <summary>
		///   Facilitates parameterized comparison of two integral values.
		/// </summary>
		/// <typeparam name="T">Type of the integer to compare.</typeparam>
		/// <param name="item1">First item in the comparison.</param>
		/// <param name="item2">Second item in the comparison.</param>
		/// <returns>True if <paramref name="item1"/> is less than <paramref name="item2"/>, false otherwise.</returns>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an integral type.</exception>
		private static bool LessThan<T>(T item1, T item2) where T : struct, IComparable, IComparable<T>, IEquatable<T>, IFormattable, IConvertible
		{
			if (typeof(T) == typeof(byte))
				return Convert.ToByte(item1) < Convert.ToByte(item2);
			if (typeof(T) == typeof(sbyte))
				return Convert.ToSByte(item1) < Convert.ToSByte(item2);
			if (typeof(T) == typeof(ushort))
				return Convert.ToUInt16(item1) < Convert.ToUInt16(item2);
			if (typeof(T) == typeof(short))
				return Convert.ToInt16(item1) < Convert.ToInt16(item2);
			if (typeof(T) == typeof(uint))
				return Convert.ToUInt32(item1) < Convert.ToUInt32(item2);
			if (typeof(T) == typeof(int))
				return Convert.ToInt32(item1) < Convert.ToInt32(item2);
			if (typeof(T) == typeof(ulong))
				return Convert.ToUInt64(item1) < Convert.ToUInt64(item2);
			if (typeof(T) == typeof(long))
				return Convert.ToInt64(item1) < Convert.ToInt64(item2);
			throw new ArgumentException("The type of T is not an integral type (byte, sbyte, ushort, short, uint, int, ulong, or long).");
		}

		#endregion
	}
}
