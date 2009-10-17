/*
 * Copyright (C) 2007-2009 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Threading;

//
// Unix system calls
//
static public class Unix
{
	[DllImport("libc")]
	static extern IntPtr localeconv ();

	[DllImport ("libc")] // Linux
	static extern int prctl (int option, byte [] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

	[DllImport ("libc")] // BSD
	static extern void setproctitle (byte [] fmt, byte [] str_arg);
 
	
	/* Taken from locale.h  */
	[StructLayout (LayoutKind.Sequential)]
	struct lconv
	{
		public string decimal_point;
		public string thousands_sep;		
		public string grouping;
		public string int_curr_symbol;
		public string currency_symbol;
		public string mon_decimal_point;
		public string mon_thousands_sep;
		public string mon_grouping;
		public string positive_sign;
		public string negative_sign;
		char int_frac_digits;
		char frac_digits;
		char p_cs_precedes;
		char p_sep_by_space;
		char n_cs_precedes;
		char n_sep_by_space;
		char p_sign_posn;
		char n_sign_posn;
		char int_p_cs_precedes;
		char int_p_sep_by_space;
		char int_n_cs_precedes;
		char int_n_sep_by_space;
		char int_p_sign_posn;
		char int_n_sign_posn;
	}

	// Mono supports less locales that Unix systems
	// To overcome this limitation we setup the right locale parameters
	// when the Mono locale is InvariantCulture, that is, when the user's locale
	// has not been identified and the default Mono locale is used
	//
	// See: https://bugzilla.novell.com/show_bug.cgi?id=420468
	// 
	static public void FixLocaleInfo ()
	{
		IntPtr st = IntPtr.Zero;
		lconv lv;
		int platform = (int) Environment.OSVersion.Platform;
		
		if (platform != 4 && platform != 128) // Only in Unix based systems
			return;

		if (CultureInfo.CurrentCulture != CultureInfo.InvariantCulture) // Culture well supported
			return;

		try {
			st = localeconv ();
			if (st == IntPtr.Zero) return;

			lv = (lconv) Marshal.PtrToStructure (st, typeof (lconv));
			CultureInfo culture =  (CultureInfo) CultureInfo.CurrentCulture.Clone ();
			culture.NumberFormat.NumberDecimalSeparator = lv.decimal_point;
			Thread.CurrentThread.CurrentCulture = culture;
		}
		catch (Exception) {}
	}

	public static void SetProcessName (string name)
	{
		int platform = (int) Environment.OSVersion.Platform;		
		if (platform != 4 && platform != 128)
			return;

		try {
			if (prctl (15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes (name + "\0"),
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0) {
				throw new ApplicationException ("Error setting process name: " + 
					Mono.Unix.Native.Stdlib.GetLastError ());
			}
		} catch (EntryPointNotFoundException) {
			setproctitle (Encoding.ASCII.GetBytes ("%s\0"), 
				Encoding.ASCII.GetBytes (name + "\0"));
		}
	}
}
