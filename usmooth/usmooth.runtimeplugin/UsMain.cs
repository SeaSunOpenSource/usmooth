/*!lic_info

The MIT License (MIT)

Copyright (c) 2015 SeaSunOpenSource

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

﻿using UnityEngine;
using System;
using GameCommon;
using usmooth.common;

public class UsMain : IDisposable
{
    public const int MAX_CONTENT_LEN = 1024;

	private ushort _serverPort = 5555;
	private long _currentTimeInMilliseconds = 0;
	private long _tickNetLast = 0;
	private long _tickNetInterval = 200;
    private LogService _logServ;

    public UsMain(bool LogRemotely, bool LogIntoFile) 
    {
		Application.runInBackground = true;

        _logServ = new LogService(LogIntoFile);

        if (LogRemotely)
        {
            _logServ.LogTargets += LogTarget_Remotely;
        }

		UsNet.Instance = new UsNet(_serverPort);
        UsvConsole.Instance = new UsvConsole();

		UsMainHandlers.Instance.RegisterHandlers(UsNet.Instance.CmdExecutor);
        UsUserCommands.Instance.RegisterHandlers(UsvConsole.Instance);
	}

    void LogTarget_Remotely(object sender, LogEventArgs args)
    {
        if (UsNet.Instance != null)
        {
            UsCmd c = new UsCmd();
            c.WriteNetCmd(eNetCmd.SV_App_Logging);
            c.WriteInt16((short)args.SeqID);
            c.WriteInt32((int)args.LogType);
            c.WriteStringStripped(args.Content, MAX_CONTENT_LEN);
            c.WriteFloat(args.Time);
            UsNet.Instance.SendCommand(c);
        }
    }

    public void Update()
    {
		_currentTimeInMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

		if (_currentTimeInMilliseconds - _tickNetLast > _tickNetInterval)
		{
			if (UsNet.Instance != null) {
				UsNet.Instance.Update ();
			}

			_tickNetLast = _currentTimeInMilliseconds;
		}
	}

    public void Dispose()
    {
        UsNet.Instance.Dispose();
        _logServ.Dispose();
    }
}