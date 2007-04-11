/*
* Copyright (c) OpenSim project, http://sim.opensecondlife.org/
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the <organization> nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY <copyright holder> ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* 
*/
using System;
using System.Collections.Generic;
using OpenSim.Framework.Console;
using OpenSim.Framework.Interfaces;
using Db4objects.Db4o;

namespace OpenUser.Config.UserConfigDb4o
{
	public class Db4oConfigPlugin: IUserConfig
	{
		public UserConfig GetConfigObject()
		{
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Loading Db40Config dll");
			return ( new DbUserConfig());
		}
	}
	
	public class DbUserConfig : UserConfig
	{
		private IObjectContainer db;	
		
		public void LoadDefaults() {
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Config.cs:LoadDefaults() - Please press enter to retain default or enter new settings");
			
			this.DefaultStartupMsg = OpenSim.Framework.Console.MainConsole.Instance.CmdPrompt("Default startup message", "Welcome to OGS");

			this.GridServerURL = OpenSim.Framework.Console.MainConsole.Instance.CmdPrompt("Grid server URL");
            		this.GridSendKey = OpenSim.Framework.Console.MainConsole.Instance.CmdPrompt("Key to send to grid server");
            		this.GridRecvKey = OpenSim.Framework.Console.MainConsole.Instance.CmdPrompt("Key to expect from grid server");
		}

		public override void InitConfig() {
			try {
				db = Db4oFactory.OpenFile("openuser.yap");
				IObjectSet result = db.Get(typeof(DbUserConfig));
				if(result.Count==1) {
					OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Config.cs:InitConfig() - Found a UserConfig object in the local database, loading");
					foreach (DbUserConfig cfg in result) {
						this.GridServerURL=cfg.GridServerURL;
						this.GridSendKey=cfg.GridSendKey;
						this.GridRecvKey=cfg.GridRecvKey;
						this.DefaultStartupMsg=cfg.DefaultStartupMsg;
					}
				} else {
					OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Config.cs:InitConfig() - Could not find object in database, loading precompiled defaults");
					LoadDefaults();
					OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Writing out default settings to local database");
					db.Set(this);
					db.Close();
				}
			} catch(Exception e) {
				OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Config.cs:InitConfig() - Exception occured");
				OpenSim.Framework.Console.MainConsole.Instance.WriteLine(e.ToString());
			}
			
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("User settings loaded:");
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Default startup message: " + this.DefaultStartupMsg);
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Grid server URL: " + this.GridServerURL);
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Key to send to grid: " + this.GridSendKey);
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Key to expect from sims: " + this.GridRecvKey);
		}
	

		public void Shutdown() {
			db.Close();
		}
	}
	
}
