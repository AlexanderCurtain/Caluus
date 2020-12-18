using System;
using System.IO;
using OpenTK;
using System.Collections.Generic;

public class RawData
{
	float[] _vertices;
	int faces = 0;

	public RawData(string OBJLocation)
	{
		var ObjFile = File.ReadLines(OBJLocation);

		List<Vector3> Vertexs = new List<Vector3>();
		List<Vector2> UVs = new List<Vector2>();
		List<Vector3> Normals = new List<Vector3>();

		foreach (string Line in ObjFile)
		{
			string[] Currentline = Line.Split(' ');
			switch (Line[0] + Line[1])
            {
				case 'v' + ' ':
					Vertexs.Add(Vector3FromString(Line));
					continue;
				case 'v' + 'n':
					Normals.Add(Vector3FromString(Line));
					continue;
				case 'v' + 't':
					UVs.Add(Vector2FromString(Line));
					continue;
				case 'f' + ' ':
					break;
			}
		}
		Vector3[] Arr_Vertexs = Vertexs.ToArray();
		Vector3[] Arr_Norms = Normals.ToArray();
		Vector2[] Arr_UV = UVs.ToArray();
		List<float> FloatList = new List<float>();
		foreach (string Line in ObjFile)
		{
			 if (Line.StartsWith("f "))
			{
				FloatList = ToFloatArray(Line, Arr_Vertexs, Arr_UV, Arr_Norms, FloatList);
				faces++;
			}
		}

		_vertices = FloatList.ToArray();
	}

	public float[] GetVertices()
	{
		return _vertices;
	}
	public int GetFaceCount()
	{
		return faces;
	}

	private static Vector3 Vector3FromString(string Line)
	{
		string[] NewLine = Line.Split(' ');
		Vector3 Vector = new Vector3(float.Parse(NewLine[1]), float.Parse(NewLine[2]), float.Parse(NewLine[3]));
		return Vector;
	}

	private static Vector2 Vector2FromString(string Line)
	{
		string[] NewLine = Line.Split(' ');
		Vector2 Vector = new Vector2(float.Parse(NewLine[1]), float.Parse(NewLine[2]));
		return Vector;
	}
	

	private static List<float> ToFloatArray(string Line, Vector3[] Vertexs, Vector2[] UVs, Vector3[] Normals, List<float> Output)
	{
		string[] SplitLine = Line.Split(' ');

		List<float> Face = new List<float>();

		Face.AddRange(AddFace(SplitLine[1], Vertexs, UVs, Normals));
		Face.AddRange(AddFace(SplitLine[2], Vertexs, UVs, Normals));
		Face.AddRange(AddFace(SplitLine[3], Vertexs, UVs, Normals));

		Face = CaluateTangent(Face);

		Output.AddRange(Face);
		return Output;

	}
	private static List<float> AddFace(string FaceLine, Vector3[] Vertexs, Vector2[] UVs, Vector3[] Normals)
	{
		List<float> Output = new List<float>();

		string[] Face1 = FaceLine.Split('/');
		// Vertexs
		Output.Add(Vertexs[int.Parse(Face1[0]) - 1].X); Output.Add(Vertexs[int.Parse(Face1[0]) - 1].Y); Output.Add(Vertexs[int.Parse(Face1[0]) - 1].Z);
		// Normals
		Output.Add(Normals[int.Parse(Face1[2]) - 1].X); Output.Add(Normals[int.Parse(Face1[2]) - 1].Y); Output.Add(Normals[int.Parse(Face1[2]) - 1].Z);
		// UV Cords
		Output.Add(UVs[int.Parse(Face1[1]) - 1].X); Output.Add(UVs[int.Parse(Face1[1]) - 1].Y);		

		return Output;
	}

	private static List<float> CaluateTangent(List<float> Face)
    {
		//brute force sort
		int i = 0;
		Vector3[] pos = new Vector3[3];
		Vector2[] Uv = new Vector2[3];
		float previous1 = 0;
		float previous2 = 0;
		foreach (float index in Face)
        {
			switch (i)
            {
				case 2: // vertex
					pos[0] = new Vector3(previous2, previous1, index);
					break; 
				case 7: // Uv
					Uv[0] = new Vector2(previous1, index);
					break;
				case 10: // vertex
					pos[1] = new Vector3(previous2, previous1, index);
					break;
				case 15: // Uv
					Uv[1] = new Vector2(previous1, index);
					break;
				case 18: // vertex
					pos[2] = new Vector3(previous2, previous1, index);
					break;
				case 23: //Uv
					Uv[2] = new Vector2(previous1, index);
					break;

			}
			previous2 = previous1;
			previous1 = index;
			i++;
        }
		//end of brute force

		Vector3 tangent = GenerateTangent(pos, Uv);

		Face.Insert(8, tangent.X);
		Face.Insert(9 , tangent.Y);
		Face.Insert(10, tangent.Z);

		Face.Insert(19, tangent.X);
		Face.Insert(20, tangent.Y);
		Face.Insert(21, tangent.Z);

		Face.Insert(30, tangent.X);
		Face.Insert(31, tangent.Y);
		Face.Insert(32, tangent.Z);

		return Face;

	}
	private static Vector3 GenerateTangent(Vector3[] pos, Vector2[] Uv)
    {
		Vector3 Deltapos1 = pos[1] - pos[0];
		Vector3 Deltapos2 = pos[2] - pos[0];

		Vector2 DeltaUv1 = Uv[1] - Uv[0];
		Vector2 DeltaUv2 = Uv[2] - Uv[0];

		float r = 1.0f / (DeltaUv1.X * DeltaUv2.Y - DeltaUv2.X * DeltaUv1.Y);

		Vector3 tangent = (Deltapos1 * DeltaUv2.Y - Deltapos2 * DeltaUv1.Y) * r;

		tangent = Vector3.Normalize(tangent);
		return tangent;
    }

}


	


