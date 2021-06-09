using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGCore;
using SGCore.SG; 
using SGCore.Kinematics;

using System.Net;
using System.Net.Sockets;

namespace SenseGloveCs
{
	class Program
	{
		static void Main(string[] args)
		{
			UdpClient client = new UdpClient();
			client.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.177"), 8080));
			
			Console.WriteLine("Testing " + SGCore.Library.Version);
			Console.WriteLine("=======================================");

			if (SGCore.DeviceList.SenseCommRunning()) //check if the Sense Comm is running. If not, warn the end user.
			{
				SGCore.SG.SenseGlove testGloveL;
				SGCore.SG.SenseGlove testGloveR;
				
				SenseGlove.GetSenseGloves(true);    
				
				if (SGCore.SG.SenseGlove.GetSenseGlove(true,out testGloveR)) //retrieves the first Sense Glove it can find. Returns true if one can be found
				{ 
				Console.Write("Right-handed SenseGlove calibration : begin.");
				Console.WriteLine();
				Console.WriteLine("Step 1: Place your hand on a flat surface, like a table, and spread your thumb and fingers.");
				Console.WriteLine("Once your hand is in the right position, press any key to continue");
				Console.ReadKey();
				testGloveR.UpdateCalibrationRange();
				Console.WriteLine("Step 2: Close your hand into a fist. Make sure your fingers aren't wrapped around your thumb.");
				Console.WriteLine("Once your hand is in the right position, press any key to continue");
				Console.ReadKey();
				SGCore.Kinematics.Vect3D[] minRangeR, maxRangeR;
				testGloveR.GetCalibrationRange(out minRangeR, out maxRangeR);
				testGloveR.UpdateCalibrationRange();
				SGCore.SG.SG_HandProfile HRP = SGCore.SG.SG_HandProfile.Default(true);
				testGloveR.ApplyCalibration(ref HRP);
				}
				
				if (SGCore.SG.SenseGlove.GetSenseGlove(false, out testGloveL)) //retrieves the first Sense Glove it can find. Returns true if one can be found
				{
				Console.Write("Left-handed SenseGlove calibration : begin.");
				Console.WriteLine();
				Console.WriteLine("Step 1: Place your hand on a flat surface, like a table, and spread your thumb and fingers.");
				Console.WriteLine("Once your hand is in the right position, press any key to continue");
				Console.ReadKey();
				testGloveL.UpdateCalibrationRange();
				Console.WriteLine("Step 2: Close your hand into a fist. Make sure your fingers aren't wrapped around your thumb.");
				Console.WriteLine("Once your hand is in the right position, press any key to continue");
				Console.ReadKey();
				SGCore.Kinematics.Vect3D[] minRangeL, maxRangeL;
				testGloveL.GetCalibrationRange(out minRangeL, out maxRangeL);
				testGloveL.UpdateCalibrationRange();
				SGCore.SG.SG_HandProfile HLP = SGCore.SG.SG_HandProfile.Default(false);
				testGloveL.ApplyCalibration(ref HLP);
				}
				
				do
				{
					SGCore.SG.SG_SensorData HLSD;
					SGCore.SG.SG_SensorData HRSD;

					
					if (testGloveR.GetSensorData(out HRSD)) //if GetSensorData is true, we have sucesfully recieved data
					{
						Console.WriteLine("Right hand");
						string rightData = (HRSD.ToString());
						System.Threading.Thread.Sleep(5000);
						
						if (rightData != null)
                        {
							byte[] dataFromR = Encoding.ASCII.GetBytes(rightData);
							client.Send(dataFromR, dataFromR.Length);
							Console.WriteLine("Data from right hand sent with success");
						}
					}

					if (testGloveL.GetSensorData(out HLSD)) //if GetSensorData is true, we have sucesfully recieved data
					{
						Console.WriteLine("Left hand");
						string leftData = (HLSD.ToString());
						System.Threading.Thread.Sleep(5000);
						if (leftData != null)
						{
							byte[] dataFromL = Encoding.ASCII.GetBytes(leftData);
							client.Send(dataFromL, dataFromL.Length);
							Console.WriteLine("Data from left hand sent with success");
						}
					}

				else
					{
						Console.WriteLine("Could not retrieve a hand pose");
					}
				

				} while (SGCore.DeviceList.SenseCommRunning());
			}
			else
			{
				Console.WriteLine("No sense gloves connected to the system. Ensure the USB connection is secure, then try again.");
			}

			Console.WriteLine("=======================================");
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}
	}
}
