/*
 * Copyright (C) 2010 Jordi Mas i Hernàndez <jmas@softcatala.org>
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
using System.IO;

namespace WebForms
{
	public enum Level { DEBUG, INFO, WARN, ERROR, FATAL };

	public interface ILogger
	{
		void Log (Level lvl, string msg, params object[] args);
	}

	class NullLogger : ILogger
	{
		public void Log (Level lvl, string msg, params object[] args)
		{
		}
	}

	class ConsoleLogger : ILogger
	{
		public void Log (Level lvl, string msg, params object[] args)
		{
			msg = string.Format ("{0} [{1}]: {2}",
			                     DateTime.Now.ToString(),
			                     Enum.GetName (typeof (Level), lvl),
			                     msg);

			Console.WriteLine (msg, args);
		}
	}

	class FileLogger : ILogger
	{
		StreamWriter log;
		ConsoleLogger console;

		public FileLogger ()
		{
			try {
				string home = Environment.GetEnvironmentVariable ("HOME");
				log = File.CreateText (Path.Combine (home != null ? home: string.Empty,
					"gbrainy_web.log"));
				log.Flush ();
			} catch (IOException) {
				// FIXME: Use temp file
			}

			console = new ConsoleLogger ();
		}

		~FileLogger ()
		{
			if (log != null)
				try {
					log.Flush ();
				} catch { }
		}

		public void Log (Level lvl, string msg, params object[] args)
		{
			console.Log (lvl, msg, args);

			if (log != null) {
				msg = string.Format ("{0} [{1}]: {2}",
				                     DateTime.Now.ToString(),
				                     Enum.GetName (typeof (Level), lvl),
				                     msg);
				log.WriteLine (msg, args);
				log.Flush();
			}
		}
	}

	// This class provides a generic logging facility. By default all
	// information is written to standard out and a log file, but other
	// loggers are pluggable.
	public static class Logger
	{
		private static Level log_level = Level.INFO;

		static ILogger log_dev = new ConsoleLogger ();

		static bool muted = false;

		public static Level LogLevel
		{
			get {
				return log_level;
			}
			set {
				log_level = value;
			}
		}

		public static ILogger LogDevice
		{
			get {
				return log_dev;
			}
			set {
				log_dev = value;
			}
		}

		public static void Debug (string msg, params object[] args)
		{
			Log (Level.DEBUG, msg, args);
		}

		public static void Info (string msg, params object[] args)
		{
			Log (Level.INFO, msg, args);
		}

		public static void Warn (string msg, params object[] args)
		{
			Log (Level.WARN, msg, args);
		}

		public static void Error (string msg, params object[] args)
		{
			Log (Level.ERROR, msg, args);
		}

		public static void Fatal (string msg, params object[] args)
		{
			Log (Level.FATAL, msg, args);
		}

		public static void Log (Level lvl, string msg, params object[] args)
		{
			if (!muted && lvl >= log_level)
				log_dev.Log (lvl, msg, args);
		}

		// This is here to support the original logging, but it should be
		// considered deprecated and old code that uses it should be upgraded to
		// call one of the level specific log methods.
		public static void Log (string msg, params object[] args)
		{
			Log (Level.DEBUG, msg, args);
		}

		public static void Mute ()
		{
			muted = true;
		}

		public static void Unmute ()
		{
			muted = false;
		}
	}
}
