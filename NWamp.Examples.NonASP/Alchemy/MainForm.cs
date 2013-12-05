using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

using NWamp;
using NWamp.Alchemy;

namespace NWamp.Alchemy.Tester
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private BaseWampHost FHost;
		
		public MainForm()
		{
			InitializeComponent();
			
			FHost = new AlchemyWampHost(IPAddress.Any, 9000);
            
            FHost.RegisterFunction("http://localhost/calc#add", (double x, double y) => x + y);

            FHost.Start();
		}
	}
}
