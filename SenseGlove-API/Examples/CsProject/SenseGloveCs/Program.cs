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

		static public double Map(double value, double istart, double istop, double ostart, double ostop)
		{
			return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
		}

        static public int Restrict(int valueToRestrict)
        {
			if (valueToRestrict>180)
            {
				return 180;
            }
            else if (valueToRestrict <= 0)
				{
				return 0;
				} 
				else 
					{
						return valueToRestrict;
					}
        }
		static void Main(string[] args)
		{
			UdpClient client = new UdpClient();
			client.Connect(new IPEndPoint(IPAddress.Parse("192.168.100.63"), 8080));

			Console.WriteLine("Testing " + SGCore.Library.Version);
			Console.WriteLine("=======================================");


			if (SGCore.DeviceList.SenseCommRunning()) //check if the Sense Comm is running. If not, warn the end user.
			{
				double R2D = 180/3.1415926;
				SGCore.SG.SenseGlove testGloveL;
				SGCore.SG.SenseGlove testGloveR;

				SenseGlove.GetSenseGloves(true);

				if (SGCore.SG.SenseGlove.GetSenseGlove(true, out testGloveR)) //retrieves the first Sense Glove it can find. Returns true if one can be found
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
					testGloveR.UpdateCalibrationRange();
					SGCore.Kinematics.Vect3D[] MinRR, MaxRR;
					testGloveR.GetCalibrationRange(out MinRR, out MaxRR);
					SGCore.SG.SG_HandProfile HRP = SGCore.SG.SG_HandProfile.Default(true);
					testGloveR.ApplyCalibration(ref HRP);

					double RIMin = (MinRR[1].y) * R2D;
					double RIMax = (MaxRR[1].y) * R2D;

					double RMMin = (MinRR[2].y) * R2D;
					double RMMax = (MaxRR[2].y) * R2D;

					double RRMin = (MinRR[3].y) * R2D;
					double RRMax = (MaxRR[3].y) * R2D;

					double RPMin = (MinRR[4].y) * R2D;
					double RPMax = (MaxRR[4].y) * R2D;

					double RTMax = (MaxRR[0].y) * R2D;
					double RTMin = (MinRR[0].y) * R2D;

					double RTAMin = (MinRR[0].z) * R2D;
					double RTAMax = (MaxRR[0].z) * R2D;

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
						testGloveL.UpdateCalibrationRange();
						SGCore.Kinematics.Vect3D[] MinRL, MaxRL;
						testGloveL.GetCalibrationRange(out MinRL, out MaxRL);
						SGCore.SG.SG_HandProfile HLP = SGCore.SG.SG_HandProfile.Default(false);
						testGloveL.ApplyCalibration(ref HLP);

						double LIMin = (MinRL[1].y) * R2D;
						double LIMax = (MaxRL[1].y) * R2D;

						double LMMin = (MinRL[2].y) * R2D;
						double LMMax = (MaxRL[2].y) * R2D;

						double LRMin = (MinRL[3].y) * R2D;
						double LRMax = (MaxRL[3].y) * R2D;

						double LPMin = (MinRL[4].y) * R2D;
						double LPMax = (MaxRL[4].y) * R2D;

						double LTMax = (MaxRL[0].y) * R2D;
						double LTMin = (MinRL[0].y) * R2D;

						double LTAMin = (MinRL[0].z) * R2D;
						double LTAMax = (MaxRL[0].z) * R2D;

						do
						{
							double RFTAT, RFTAM, RFTAI, RFTAR, RFTAP, RFAT, LFTAT, LFTAM, LFTAI, LFTAR, LFTAP, LFAT;

							SGCore.SG.SG_SensorData HLSD;
							SGCore.SG.SG_SensorData HRSD;
							if (testGloveR.GetSensorData(out HRSD)) //if GetSensorData is true, we have sucesfully recieved data
							{
								float[] HRAT = HRSD.GetAngles(Finger.Thumb);
								float[] HRAI = HRSD.GetAngles(Finger.Index);
								float[] HRAM = HRSD.GetAngles(Finger.Middle);
								float[] HRAR = HRSD.GetAngles(Finger.Ring);
								float[] HRAP = HRSD.GetAngles(Finger.Pinky);


								RFAT = (HRAT[0]) * R2D;
								RFTAT = (HRAT[1] + HRAT[2] + HRAT[3]) * R2D;
								RFTAI = (HRAI[1] + HRAI[2] + HRAI[3]) * R2D;
								RFTAM = (HRAM[1] + HRAM[2] + HRAM[3]) * R2D;
								RFTAR = (HRAR[1] + HRAR[2] + HRAR[3]) * R2D;
								RFTAP = (HRAP[1] + HRAP[2] + HRAP[3]) * R2D;

						
								double mappedValueFTAI = Map(RFTAI, RIMin, RIMax, 0, 180);
								int resRIndex = (int)mappedValueFTAI;
								int finalresRIndex = Restrict(resRIndex); 

								double mappedValueFTAM = Map(RFTAM, RMMin, RMMax, 0, 180);
								int resRMid = (int)mappedValueFTAM;
								int finalresRMiddle = Restrict(resRMid);

								double mappedValueFTAR = Map(RFTAR, RRMin, RRMax, 0, 180);
								int resRRing = (int)mappedValueFTAR;
								int finalresRRing = Restrict(resRRing);

								double mappedValueFTAP = Map(RFTAP, RPMin, RPMax, 0, 180);
								int resRPinky = (int)mappedValueFTAP;
								int finalresRPinky = Restrict(resRPinky);

								double mappedValueFTAT = Map(RFTAT, RTMin, RTMax+30, 0, 180);
								int resRThump = (int)mappedValueFTAT;
								int finalresRThumb = Restrict(resRThump);

								double mappedValueFTAbdT = Map(RFAT, RTAMin, RTAMax, 0, 180);
								int resRThumpAbd = (int)mappedValueFTAbdT;
								int finalresRThumbabd = Restrict(resRThumpAbd);


								Console.WriteLine("<1.3.1."+ finalresRThumbabd + "." + finalresRThumb + "." + finalresRIndex + "." + finalresRMiddle + "." + finalresRRing + "." + finalresRPinky+">");
								string PromeRHandMov =("<1.3.1." + finalresRThumbabd + "." + finalresRThumb + "." + finalresRIndex + "." + finalresRMiddle + "." + finalresRRing + "." + finalresRPinky + ">");
								
								/*Console.WriteLine("Mapped value for Thump abduction: " + finalresRThumbabd);
								Console.WriteLine("Mapped value for Thump flexion: " + finalresRThumb);
								Console.WriteLine("Mapped value for Index flexion: " + finalresRIndex);
								Console.WriteLine("Mapped value for Midlle flexion: " + finalresRMiddle);
								Console.WriteLine("Mapped value for Ring flexion: " + finalresRRing);
								Console.WriteLine("Mapped value for Pinky flexion: " + finalresRPinky);
								*/

									//Console.WriteLine("Right hand");
									//string rightData = (HRSD.ToString());
									System.Threading.Thread.Sleep(5);
									//Console.ReadKey();

									if (PromeRHandMov != null)
									{
										//byte[] dataFromR = Encoding.ASCII.GetBytes(rightData);
										byte[] dataFromR2P = Encoding.ASCII.GetBytes(PromeRHandMov);
										client.Send(dataFromR2P, dataFromR2P.Length);
										//client.Send(dataFromR, dataFromR.Length);
										//Console.WriteLine("Data from right hand sent with success");
									} 


										if (testGloveL.GetSensorData(out HLSD)) //if GetSensorData is true, we have sucesfully recieved data
										{
											float[] HLAT = HLSD.GetAngles(Finger.Thumb);
											float[] HLAI = HLSD.GetAngles(Finger.Index);
											float[] HLAM = HLSD.GetAngles(Finger.Middle);
											float[] HLAR = HLSD.GetAngles(Finger.Ring);
											float[] HLAP = HLSD.GetAngles(Finger.Pinky);


											LFAT = (HLAT[0]) * R2D;
											LFTAT = (HLAT[1] + HLAT[2] + HLAT[3]) * R2D;
											LFTAI = (HLAI[1] + HLAI[2] + HLAI[3]) * R2D;
											LFTAM = (HLAM[1] + HLAM[2] + HLAM[3]) * R2D;
											LFTAR = (HLAR[1] + HLAR[2] + HLAR[3]) * R2D;
											LFTAP = (HLAP[1] + HLAP[2] + HLAP[3]) * R2D;

											double mappedValueLFTAI = Map(LFTAI, LIMin, LIMax, 0, 180);
											int resLIndex = (int)mappedValueLFTAI;
											int finalresLIndex = Restrict(resLIndex);

											double mappedValueLFTAM = Map(LFTAM, LMMin, LMMax, 0, 180);
											int resLMid = (int)mappedValueLFTAM;
											int finalresLMiddle = Restrict(resLMid);

											double mappedValueLFTAR = Map(LFTAR, LRMin, LRMax, 0, 180);
											int resLRing = (int)mappedValueLFTAR;
											int finalresLRing = Restrict(resLRing);

											double mappedValueLFTAP = Map(LFTAP, LPMin, LPMax, 0, 180);
											int resLPinky = (int)mappedValueLFTAP;
											int finalresLPinky = Restrict(resLPinky);

											double mappedValueLFTAT = Map(LFTAT, LTMin, LTMax +70, 0, 180);
											int resLThump = (int)mappedValueLFTAT;
											int finalresLThumb = Restrict(resLThump);

											double mappedValueLFTAbdT = Map(-(LFAT), -(LTAMax), -(LTAMin), 0, 180);
											int resLThumpAbd = (int)mappedValueLFTAbdT;
											int finalresLThumbAbd = Restrict(resLThumpAbd);


											Console.WriteLine("<1.5.1."+ finalresLThumbAbd+ "." + finalresLThumb + "." + finalresLIndex + "." + finalresLMiddle + "." + finalresLRing + "." + finalresLPinky+">");
											string PromeLHandMov = ("<1.5.1." + finalresLThumbAbd + "." + finalresLThumb + "." + finalresLIndex + "." + finalresLMiddle + "." + finalresLRing + "." + finalresLPinky + ">");

											/*Console.WriteLine("Mapped value for Thump abduction: " + finalresLThumbAbd);
											Console.WriteLine("Mapped value for Thump flexion: " + finalresLThumb);
											Console.WriteLine("Mapped value for Index flexion: " + finalresLIndex);
											Console.WriteLine("Mapped value for Midlle flexion: " + finalresLMiddle);
											Console.WriteLine("Mapped value for Ring flexion: " + finalresLRing);
											Console.WriteLine("Mapped value for Pinky flexion: " + finalresLPinky);
											*/

											//Console.WriteLine("Left hand");
											string leftData = (HLSD.ToString());
		
											System.Threading.Thread.Sleep(5);
											if (PromeLHandMov != null)
											{
												byte[] dataFromL2P = Encoding.ASCII.GetBytes(PromeLHandMov);
												//byte[] dataFromL = Encoding.ASCII.GetBytes(leftData);
												//client.Send(dataFromL, dataFromL.Length);
												client.Send(dataFromL2P, dataFromL2P.Length);
												//Console.WriteLine("Data from left hand sent with success");
											}
								}
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
}
}
	

