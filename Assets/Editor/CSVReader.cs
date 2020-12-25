using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.NRefactory.Ast;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

public class CSVTools : Editor
{
    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string SPLIT_SPACE = @"\s+";

    private static readonly float threshold = 0.001f;
    
    [MenuItem("Tools/ReadCSV", priority = 100)]
    static void DoIt()
    {
        ResloveXML();
    }

    static void ResloveXML()
    {
        var result = CSVReader.ReadLightmapData("UE_LightmapData");
        
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/Resources/" + "LightmapData.xml");

        XmlNodeList xmlNodeList = xml.SelectSingleNode("scene").ChildNodes;
        
        foreach (XmlElement node in xmlNodeList)
        {
            CheckAndSetContent(result, node);
        }
        
        xml.Save(Application.dataPath + "/Resources/" + "LightmapData.xml");
    }

    static bool CheckAndSetContent(List<Dictionary<string, Vector4>> ueLightmapData, XmlElement ceLightmapDataNode)
    {
        Vector4 scaleOffset = ConvertStringToVector4(ceLightmapDataNode.GetAttribute("ScaleOffset"));
        Vector4 lightMapScale0 = ConvertStringToVector4(ceLightmapDataNode.GetAttribute("LightMapScale0"));
        Vector4 lightMapScale1 = ConvertStringToVector4(ceLightmapDataNode.GetAttribute("LightMapScale1"));
        Vector4 lightmapAdd0 = ConvertStringToVector4(ceLightmapDataNode.GetAttribute("LightMapAdd0"));
        Vector4 lightmapAdd1 = ConvertStringToVector4(ceLightmapDataNode.GetAttribute("LightMapAdd1"));

        foreach (var lightmapDataDic in ueLightmapData)
        {
            if ((lightmapDataDic["LightMapCoordinateScaleBias"] - scaleOffset).SqrMagnitude() < threshold &&
                (lightmapDataDic["LightMapScale0"] - lightMapScale0).SqrMagnitude() < threshold &&
                (lightmapDataDic["LightMapScale1"] - lightMapScale1).SqrMagnitude() < threshold &&
                (lightmapDataDic["LightMapAdd0"] - lightmapAdd0).SqrMagnitude() < threshold && 
                (lightmapDataDic["LightMapAdd1"] - lightmapAdd1).SqrMagnitude() < threshold)
            {
                var ueShadowMapCoordinateScaleBias = lightmapDataDic["ShadowMapCoordinateScaleBias"];
                ceLightmapDataNode.SetAttribute("ShadowMapCoordinateScaleBias",
                    string.Format("{0} {1} {2} {3}", ueShadowMapCoordinateScaleBias.x, ueShadowMapCoordinateScaleBias.y,
                        ueShadowMapCoordinateScaleBias.z, ueShadowMapCoordinateScaleBias.w));

                var ueStaticShadowMapMasks = lightmapDataDic["StaticShadowMapMasks"];
                ceLightmapDataNode.SetAttribute("StaticShadowMapMasks",
                    string.Format("{0} {1} {2} {3}", ueStaticShadowMapMasks.x, ueStaticShadowMapMasks.y,
                        ueStaticShadowMapMasks.z, ueStaticShadowMapMasks.w));

                var ueInvUniformPenumbraSizes = lightmapDataDic["InvUniformPenumbraSizes"];
                ceLightmapDataNode.SetAttribute("InvUniformPenumbraSizes",
                    string.Format("{0} {1} {2} {3}", ueInvUniformPenumbraSizes.x, ueInvUniformPenumbraSizes.y,
                        ueInvUniformPenumbraSizes.z, ueInvUniformPenumbraSizes.w));

                var ueLightmapVTPackedPageTableUniform0 = lightmapDataDic["LightmapVTPackedPageTableUniform0"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedPageTableUniform0",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedPageTableUniform0.x, ueLightmapVTPackedPageTableUniform0.y,
                        ueLightmapVTPackedPageTableUniform0.z, ueLightmapVTPackedPageTableUniform0.w));
                
                var ueLightmapVTPackedPageTableUniform1 = lightmapDataDic["LightmapVTPackedPageTableUniform1"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedPageTableUniform1",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedPageTableUniform1.x, ueLightmapVTPackedPageTableUniform1.y,
                        ueLightmapVTPackedPageTableUniform1.z, ueLightmapVTPackedPageTableUniform1.w));

                var ueLightmapVTPackedUniform0 = lightmapDataDic["LightmapVTPackedUniform0"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedUniform0",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedUniform0.x, ueLightmapVTPackedUniform0.y,
                        ueLightmapVTPackedUniform0.z, ueLightmapVTPackedUniform0.w));
                
                var ueLightmapVTPackedUniform1 = lightmapDataDic["LightmapVTPackedUniform1"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedUniform1",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedUniform1.x, ueLightmapVTPackedUniform1.y,
                        ueLightmapVTPackedUniform1.z, ueLightmapVTPackedUniform1.w));
                
                var ueLightmapVTPackedUniform2 = lightmapDataDic["LightmapVTPackedUniform2"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedUniform2",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedUniform2.x, ueLightmapVTPackedUniform2.y,
                        ueLightmapVTPackedUniform2.z, ueLightmapVTPackedUniform2.w));
                
                var ueLightmapVTPackedUniform3 = lightmapDataDic["LightmapVTPackedUniform3"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedUniform3",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedUniform3.x, ueLightmapVTPackedUniform3.y,
                        ueLightmapVTPackedUniform3.z, ueLightmapVTPackedUniform3.w));
                
                var ueLightmapVTPackedUniform4 = lightmapDataDic["LightmapVTPackedUniform4"];
                ceLightmapDataNode.SetAttribute("LightmapVTPackedUniform4",
                    string.Format("{0} {1} {2} {3}", ueLightmapVTPackedUniform4.x, ueLightmapVTPackedUniform4.y,
                        ueLightmapVTPackedUniform4.z, ueLightmapVTPackedUniform4.w));

                ceLightmapDataNode.SetAttribute("ShadowMapIndex", "0");
            }
        }

        return true;
    }

    static Vector4 ConvertStringToVector4(string strVec4)
    {
        Vector4 vec;
        
        Debug.Assert(strVec4 != "");

        strVec4 = strVec4.TrimStart().TrimEnd();
        
        var strList = Regex.Split(strVec4, SPLIT_SPACE);
        if (strList.Length != 4)
        {
            Debug.Log(strVec4);
            return Vector4.zero;
        }
        
        vec.x = float.Parse(strList[0]);
        vec.y = float.Parse(strList[1]);
        vec.z = float.Parse(strList[2]);
        vec.w = float.Parse(strList[3]);
        
        return vec;
    }
}

// 来源：https://github.com/tiago-peres/blog/blob/master/csvreader
public class CSVReader
{
    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static char[] TRIM_CHARS = {'\"'};

    private static string[] lightmapDataKey =
    {
        "StaticShadowMapMasks",
            "InvUniformPenumbraSizes",
            "LightMapCoordinateScaleBias",
            "ShadowMapCoordinateScaleBias",
            "LightMapScale0",
            "LightMapScale1",
            "LightMapAdd0",
            "LightMapAdd1",
            "LightmapVTPackedPageTableUniform0",
            "LightmapVTPackedPageTableUniform1",
            "LightmapVTPackedUniform0",
            "LightmapVTPackedUniform1",
            "LightmapVTPackedUniform2",
            "LightmapVTPackedUniform3",
            "LightmapVTPackedUniform4"
    };

    public static List<Dictionary<string, Vector4>> ReadLightmapData(string file)
    {
        var resultList = new List<Dictionary<string, Vector4>>();

        TextAsset csvContent = Resources.Load<TextAsset>(file);
        var lines = Regex.Split(csvContent.text, LINE_SPLIT_RE);
        Debug.Log("lines = " + lines.Length);
        if (lines.Length <= 1)
            return resultList;

        var header = Regex.Split(lines[0], SPLIT_RE);
        int i = 1;
        while(i < lines.Length - 1)
        {
            var keyDict = new Dictionary<string, Vector4>();
            foreach (var key in lightmapDataKey)
            {
                var values = Regex.Split(lines[i++], SPLIT_RE);

                Vector4 value;
                float f;
                float.TryParse(values[1], out f);
                value.x = f;
                
                float.TryParse(values[2], out f);
                value.y = f;
                
                float.TryParse(values[3], out f);
                value.z = f;
                
                float.TryParse(values[4], out f);
                value.w = f;
                
                keyDict.Add(key, value);
            }
            
            resultList.Add(keyDict);
        }

        return resultList;
    }
}

