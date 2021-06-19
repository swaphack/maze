using System.Collections;
using System;
using System.IO;
using System.Reflection;

public class Packet
{
	private BinaryWriter _writer;

	private BinaryReader _reader;

	public Packet(Stream stream)
	{
		_writer = new BinaryWriter (stream);
		_reader = new BinaryReader (stream);
	}

	public void Write(object target)
	{
		if (_writer == null || target == null) {
			return;
		}
		var type = target.GetType ();
		var members = type.GetMembers ();
		for (var i = 0; i < members.Length; i++) {
			var t = members [i].GetType ();
			var field = t.GetField (t.Name);
			var code = Type.GetTypeCode (t);
			switch (code) {
			case TypeCode.Boolean: _writer.Write ((Boolean)target); break;
			case TypeCode.Char: _writer.Write ((Char)target); break;
			case TypeCode.SByte: _writer.Write ((SByte)target); break;
			case TypeCode.Byte: _writer.Write ((Byte)target); break;
			case TypeCode.Int16: _writer.Write ((Int16)target); break;
			case TypeCode.UInt16: _writer.Write ((UInt16)target); break;
			case TypeCode.Int32: _writer.Write ((Int32)target); break;
			case TypeCode.UInt32: _writer.Write ((UInt32)target); break;
			case TypeCode.Int64: _writer.Write ((Int64)target); break;
			case TypeCode.UInt64: _writer.Write ((UInt64)target); break;
			case TypeCode.Single: _writer.Write ((Single)target); break;
			case TypeCode.Double: _writer.Write ((Double)target); break;
			case TypeCode.String: _writer.Write ((String)target); break;
			default: 
				{
					var obj = target as ICollection;
					if (obj != null) {
						WriteCollect (target as ICollection);
					}
				}
				break;
			}
		}

	}

	protected void WriteCollect(ICollection target)
	{
		if (_writer == null || target == null) {
			return;
		}

		var list = target as IList;
		if (list != null) {
			Write (list.Count);
			for (var i = 0; i < list.Count; i++) {
				Write (list[i]);
			}
		}

		var dict = target as IDictionary;
		if (dict != null) {
			Write (dict.Count);
			var e = dict.GetEnumerator();
			while (e != null) {
				Write (e.Key);
				Write (e.Value);
				e.MoveNext ();
			}
		}
	}

	public void Read(ref object target)
	{
		if (_reader == null || target == null) {
			return;
		}

		var type = target.GetType ();
		var members = type.GetMembers ();
		for (var i = 0; i < members.Length; i++) {
			var t = target.GetType ();
			var code = Type.GetTypeCode (t);
			switch (code) {
			case TypeCode.Boolean: target = _reader.ReadBoolean (); break;
			case TypeCode.Char: target = _reader.ReadChar (); break;
			case TypeCode.SByte: target = _reader.ReadSByte (); break;
			case TypeCode.Byte: target = _reader.ReadByte (); break;
			case TypeCode.Int16: target = _reader.ReadInt16 (); break;
			case TypeCode.UInt16: target = _reader.ReadUInt16 (); break;
			case TypeCode.Int32: target = _reader.ReadInt32 (); break;
			case TypeCode.UInt32: target = _reader.ReadUInt32 (); break;
			case TypeCode.Int64: target = _reader.ReadInt64 (); break;
			case TypeCode.UInt64: target = _reader.ReadUInt64 (); break;
			case TypeCode.Single: target = _reader.ReadSingle (); break;
			case TypeCode.Double: target = _reader.ReadDouble (); break;
			case TypeCode.String: target = _reader.ReadString (); break;
			default: 
				{
					var obj = target as ICollection;
					if (obj != null) {
						ReadCollect (target as ICollection);
					}
				}
				break;
			}	
		}
	}

	protected void ReadCollect(ICollection target)
	{
		if (_reader == null || target == null) {
			return;
		}
		/*
		var list = target as IList;
		if (list != null) {
			int count = 0;
			Read (ref count as object);
			for (var i = 0; i < count; i++) {
				Read (list[i]);
			}
		}

		var dict = target as IDictionary;
		if (dict != null) {
			int count = 0;
			Read (ref count as object);
			for (var i = 0; i < count; i++) {
				Write (keys[i]);
				Write (dict [keys [i]]);
			}
		}
		*/
	}
}

