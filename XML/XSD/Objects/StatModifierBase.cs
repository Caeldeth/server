// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code++. Version 5.1.46.0. www.xsd2code.com
//  </auto-generated>
// ------------------------------------------------------------------------------
#pragma warning disable
namespace Hybrasyl.Xml
{
using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Collections.Generic;

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
[Serializable]
[DebuggerStepThrough]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[XmlTypeAttribute(Namespace="http://www.hybrasyl.com/XML/Hybrasyl/2020-02")]
public partial class StatModifierBase
{
    #region Private fields
    private sbyte _str;
    private sbyte _int;
    private sbyte _wis;
    private sbyte _con;
    private sbyte _dex;
    private int _hp;
    private int _mp;
    private static XmlSerializer serializer;
    #endregion
    
    public StatModifierBase()
    {
        _str = ((sbyte)(0));
        _int = ((sbyte)(0));
        _wis = ((sbyte)(0));
        _con = ((sbyte)(0));
        _dex = ((sbyte)(0));
        _hp = 0;
        _mp = 0;
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(sbyte), "0")]
    public sbyte Str
    {
        get
        {
            return _str;
        }
        set
        {
            _str = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(sbyte), "0")]
    public sbyte Int
    {
        get
        {
            return _int;
        }
        set
        {
            _int = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(sbyte), "0")]
    public sbyte Wis
    {
        get
        {
            return _wis;
        }
        set
        {
            _wis = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(sbyte), "0")]
    public sbyte Con
    {
        get
        {
            return _con;
        }
        set
        {
            _con = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(typeof(sbyte), "0")]
    public sbyte Dex
    {
        get
        {
            return _dex;
        }
        set
        {
            _dex = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(0)]
    public int Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
        }
    }
    
    [XmlAttribute]
    [DefaultValue(0)]
    public int Mp
    {
        get
        {
            return _mp;
        }
        set
        {
            _mp = value;
        }
    }
    
    private static XmlSerializer Serializer
    {
        get
        {
            if ((serializer == null))
            {
                serializer = new XmlSerializerFactory().CreateSerializer(typeof(StatModifierBase));
            }
            return serializer;
        }
    }
    
    #region Serialize/Deserialize
    /// <summary>
    /// Serializes current StatModifierBase object into an XML string
    /// </summary>
    /// <returns>string XML value</returns>
    public virtual string Serialize()
    {
        StreamReader streamReader = null;
        MemoryStream memoryStream = null;
        try
        {
            memoryStream = new MemoryStream();
            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = "  ";
            System.Xml.XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            Serializer.Serialize(xmlWriter, this);
            memoryStream.Seek(0, SeekOrigin.Begin);
            streamReader = new StreamReader(memoryStream);
            return streamReader.ReadToEnd();
        }
        finally
        {
            if ((streamReader != null))
            {
                streamReader.Dispose();
            }
            if ((memoryStream != null))
            {
                memoryStream.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Deserializes workflow markup into an StatModifierBase object
    /// </summary>
    /// <param name="input">string workflow markup to deserialize</param>
    /// <param name="obj">Output StatModifierBase object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool Deserialize(string input, out StatModifierBase obj, out Exception exception)
    {
        exception = null;
        obj = default(StatModifierBase);
        try
        {
            obj = Deserialize(input);
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
    
    public static bool Deserialize(string input, out StatModifierBase obj)
    {
        Exception exception = null;
        return Deserialize(input, out obj, out exception);
    }
    
    public static StatModifierBase Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return ((StatModifierBase)(Serializer.Deserialize(XmlReader.Create(stringReader))));
        }
        finally
        {
            if ((stringReader != null))
            {
                stringReader.Dispose();
            }
        }
    }
    
    public static StatModifierBase Deserialize(Stream s)
    {
        return ((StatModifierBase)(Serializer.Deserialize(s)));
    }
    #endregion
    
    /// <summary>
    /// Serializes current StatModifierBase object into file
    /// </summary>
    /// <param name="fileName">full path of outupt xml file</param>
    /// <param name="exception">output Exception value if failed</param>
    /// <returns>true if can serialize and save into file; otherwise, false</returns>
    public virtual bool SaveToFile(string fileName, out Exception exception)
    {
        exception = null;
        try
        {
            SaveToFile(fileName);
            return true;
        }
        catch (Exception e)
        {
            exception = e;
            return false;
        }
    }
    
    public virtual void SaveToFile(string fileName)
    {
        StreamWriter streamWriter = null;
        try
        {
            string xmlString = Serialize();
            FileInfo xmlFile = new FileInfo(fileName);
            streamWriter = xmlFile.CreateText();
            streamWriter.WriteLine(xmlString);
            streamWriter.Close();
        }
        finally
        {
            if ((streamWriter != null))
            {
                streamWriter.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Deserializes xml markup from file into an StatModifierBase object
    /// </summary>
    /// <param name="fileName">string xml file to load and deserialize</param>
    /// <param name="obj">Output StatModifierBase object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool LoadFromFile(string fileName, out StatModifierBase obj, out Exception exception)
    {
        exception = null;
        obj = default(StatModifierBase);
        try
        {
            obj = LoadFromFile(fileName);
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
    
    public static bool LoadFromFile(string fileName, out StatModifierBase obj)
    {
        Exception exception = null;
        return LoadFromFile(fileName, out obj, out exception);
    }
    
    public static StatModifierBase LoadFromFile(string fileName)
    {
        FileStream file = null;
        StreamReader sr = null;
        try
        {
            file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(file);
            string xmlString = sr.ReadToEnd();
            sr.Close();
            file.Close();
            return Deserialize(xmlString);
        }
        finally
        {
            if ((file != null))
            {
                file.Dispose();
            }
            if ((sr != null))
            {
                sr.Dispose();
            }
        }
    }
}
}
#pragma warning restore
