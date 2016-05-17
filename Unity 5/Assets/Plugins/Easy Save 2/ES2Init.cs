
using System; 
using UnityEngine; 
using System.Collections; 
using System.Collections.Generic;
				
public class ES2Init : MonoBehaviour
{
	public void Awake()
	{
		ES2TypeManager.types = new Dictionary<Type, ES2Type>
		{
{typeof(System.Boolean), new ES2_bool()},{typeof(System.Byte), new ES2_byte()},{typeof(System.Char), new ES2_char()},{typeof(System.Decimal), new ES2_decimal()},{typeof(System.Double), new ES2_double()},{typeof(System.Single), new ES2_float()},{typeof(System.Int32), new ES2_int()},{typeof(System.Int64), new ES2_long()},{typeof(System.Int16), new ES2_short()},{typeof(System.String), new ES2_string()},{typeof(System.SByte), new ES2_sbyte()},{typeof(System.UInt32), new ES2_uint()},{typeof(System.UInt64), new ES2_ulong()},{typeof(System.UInt16), new ES2_ushort()},{typeof(UnityEngine.Vector2), new ES2_Vector2()},{typeof(UnityEngine.Vector3), new ES2_Vector3()},{typeof(UnityEngine.Vector4), new ES2_Vector4()},{typeof(UnityEngine.Transform), new ES2_Transform()},{typeof(UnityEngine.Texture2D), new ES2_Texture2D()},{typeof(UnityEngine.Quaternion), new ES2_Quaternion()},{typeof(UnityEngine.BoxCollider), new ES2_BoxCollider()},{typeof(UnityEngine.Mesh), new ES2_Mesh()},{typeof(UnityEngine.Color), new ES2_Color()},{typeof(UnityEngine.CapsuleCollider), new ES2_CapsuleCollider()},{typeof(UnityEngine.SphereCollider), new ES2_SphereCollider()},{typeof(UnityEngine.MeshCollider), new ES2_MeshCollider()},{typeof(UnityEngine.AudioClip), new ES2_AudioClip()},{typeof(UnityEngine.Color32), new ES2_Color32()},{typeof(UnityEngine.Material), new ES2_Material()},{typeof(UnityEngine.Rect), new ES2_Rect()},{typeof(UnityEngine.Bounds), new ES2_Bounds()}		};
	}
}