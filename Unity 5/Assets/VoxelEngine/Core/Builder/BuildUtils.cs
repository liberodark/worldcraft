using UnityEngine;
using System.Collections;

class BuildUtils {
	
	private static Color ComputeSmoothLight(Map map, Vector3i a, Vector3i b, Vector3i c, Vector3i d) {
		if(map.GetBlock(b).IsAlpha() || map.GetBlock(c).IsAlpha()) {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			Color c4 = GetBlockLight(map, d);
			return (c1 + c2 + c3 + c4)/4f;
		} else {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			return (c1 + c2 + c3)/3f;
		}
	}
	
	public static Color GetSmoothVertexLight(Map map, Vector3i pos, Vector3 vertex, CubeFace face) {
		// pos - позиция блока 
		// vertex позиция вершины относительно блока т.е. от -0.5 до 0.5
		int dx = (int)Mathf.Sign( vertex.x );
		int dy = (int)Mathf.Sign( vertex.y );
		int dz = (int)Mathf.Sign( vertex.z );
		
		Vector3i a, b, c, d;
		if(face == CubeFace.Left || face == CubeFace.Right) { // X
			a = pos + new Vector3i(dx, 0,  0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(dx, 0,  dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else 
		if(face == CubeFace.Bottom || face == CubeFace.Top) { // Y
			a = pos + new Vector3i(0,  dy, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else { // Z
			a = pos + new Vector3i(0,  0,  dz);
			b = pos + new Vector3i(dx, 0,  dz);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		}
		
		if(map.GetBlock(b).IsAlpha() || map.GetBlock(c).IsAlpha()) {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			Color c4 = GetBlockLight(map, d);
			return (c1 + c2 + c3 + c4)/4f;
		} else {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			return (c1 + c2 + c3)/3f;
		}
	}
	
	public static Color GetBlockLight(Map map, Vector3i pos) {
		Vector3i chunkPos = Chunk.ToChunkPosition(pos);
		Vector3i localPos = Chunk.ToLocalPosition(pos);
		float light = (float) map.GetLightmap().GetLight( chunkPos, localPos ) / SunLightComputer.MAX_LIGHT;
		float sun = (float) map.GetSunLightmap().GetLight( chunkPos, localPos, pos.y ) / SunLightComputer.MAX_LIGHT;
		return new Color(light, light, light, sun);
	}

	public static void calculateMeshTangents(Mesh mesh)
	{
		//speed up math by copying the mesh arrays
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		
		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		Vector4[] tangents = new Vector4[vertexCount];
		
		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		
		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			
			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		mesh.tangents = tangents;
	}
	
}