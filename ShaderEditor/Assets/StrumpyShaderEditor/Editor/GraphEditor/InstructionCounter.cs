using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System;

namespace StrumpyShaderEditor {
	public struct InstructionCount {
		public int ALU;
		public int V_ALU; // Vertex ALU
		public int TEX; 
		
		public InstructionCount (int alu, int valu, int tex) { ALU = alu; V_ALU = valu; TEX = tex; }
	}
	
	public struct ProgramCount {
		public InstructionCount GL;
		public InstructionCount D3D;
		public ProgramCount(InstructionCount gl, InstructionCount dx) { GL = gl; D3D = dx; }
	}
	
	enum ParseAPI { D3D, GL, NA }
	enum ParseStage { Vertex, Fragment, NA }
	
	
	
	public static class InstructionCounter { 
		public static ProgramCount lastCount = new ProgramCount(new InstructionCount(-1,-1,-1),new InstructionCount(-1,-1,-1));
		
		//[MenuItem("File/Count Instructions")]
		public static void CountInstructions() {
			string assetsDir = Application.dataPath;
			string tempDir = Path.Combine(Path.GetDirectoryName(assetsDir),"Temp");
			
			const string fileName = "CgBatchOutput.shader";
			
			string filePath = Path.Combine(tempDir,fileName);
			
			//Debug.Log(filePath);
			
			int GL_ALU = -1;
			int GL_V_ALU = -1;
			int GL_TEX = -1;
			
			int D3D_ALU = -1;
			int D3D_V_ALU = -1;
			int D3D_TEX = -1;
			
			try {
				var api = ParseAPI.NA;
				var stage = ParseStage.NA;
				
				bool hitFive = false; // Shoddy catch for the ALU-5 program
				
				using (StreamReader sr = new StreamReader(filePath)) {
					String line;
					
					while ((line = sr.ReadLine()) != null) {
						// Set parse stages
						if (line.Contains("Program \"vp\""))
							stage = ParseStage.Vertex;
						
						if (line.Contains("Program \"fp\""))
							stage = ParseStage.Fragment;
							
						if (line.Contains("opengl"))
							api = ParseAPI.GL;
						
						if (line.Contains("d3d9"))
							api = ParseAPI.D3D;
						
						// Parse for the ALU Counts
						
						if (line.Contains("ALU:")) {
							string subLine = line;
							subLine = subLine.Substring(subLine.IndexOf("to ") + 3);
							//Debug.Log(subLine);
							
							int ALUFound;	
							bool canParse = int.TryParse(subLine.Split(new char[2] { " "[0], ","[0] } )[0],out ALUFound);
							
							if (!canParse)
								ALUFound = -1; 
							
							// Shoddy catch for the 5ALU intermediary program
							if (ALUFound == 5)
								if (hitFive == true)
									ALUFound = -1; 
							
							switch (stage) {
								case ParseStage.Vertex : {
									switch (api) {
										case ParseAPI.GL : {
											GL_V_ALU = Mathf.Max(ALUFound,GL_V_ALU);
											break;
										}
										
										case ParseAPI.D3D : {
											D3D_V_ALU = Mathf.Max(ALUFound, D3D_V_ALU);
											break;
										}
									}
									break;
								}
								
								case ParseStage.Fragment : {
									switch (api) {
										case ParseAPI.GL : {
											GL_ALU = Mathf.Max(ALUFound,GL_ALU);
											break;
										}
										
										case ParseAPI.D3D : {
											D3D_ALU = Mathf.Max(ALUFound, D3D_ALU);
											break;
										}
									}
									break;
								}
							}
						}
						
						// Parse for TEX Counts
						
						if (line.Contains(", TEX:")) {
							string subLine = line;
							subLine = subLine.Substring(subLine.IndexOf("TEX:")); // Cut out the preliminary ALU listing
							subLine = subLine.Substring(subLine.IndexOf("to ") + 3); // Grab the second value
							subLine = subLine.Split(" "[0])[0]; // Cut anything that snuck in after
							
							//Debug.Log("Tex Count - " + subLine);
							
							int TEXFound;
							int.TryParse(subLine,out TEXFound);
							
							switch (api) {
								case ParseAPI.GL : {
									GL_TEX = Mathf.Max(TEXFound,GL_TEX);
									break;
								}
										
								case ParseAPI.D3D : {
									D3D_TEX = Mathf.Max(TEXFound, D3D_TEX);
									break;
								}
							}
							
						}
					}
				}
			} catch (Exception e) { Debug.Log("Shader parsing error " + e); } // Silent fail 
			
			var GL = new InstructionCount(GL_ALU,GL_V_ALU,GL_TEX);
			var D3D = new InstructionCount(D3D_ALU,D3D_V_ALU,D3D_TEX);
			
			lastCount = new ProgramCount(GL,D3D);
			
			//Debug.Log("GL Count: " + GL_ALU.ToString() + "ALU, " + GL_V_ALU.ToString() + "V_ALU, " + GL_TEX + "TEX");
		}
	}

}
