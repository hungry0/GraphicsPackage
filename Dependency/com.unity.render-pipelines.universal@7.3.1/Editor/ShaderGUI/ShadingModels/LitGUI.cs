﻿using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    [MovedFrom("UnityEditor.Rendering.LWRP.ShaderGUI")] public static class LitGUI
    {
        public enum WorkflowMode
        {
            Specular = 0,
            Metallic
        }

        public enum SmoothnessMapChannel
        {
            SpecularMetallicAlpha,
            AlbedoAlpha,
        }

        public static class Styles
        {
            public static GUIContent workflowModeText = new GUIContent("Workflow Mode",
                "Select a workflow that fits your textures. Choose between Metallic or Specular.");
            
            public static GUIContent clearCoatText = new GUIContent("Clear Coat", "Clear Coat.");
            
            public static GUIContent clearCoatRoughnessText = new GUIContent("Clear Coat Roughness", "CCR");
            
            public static GUIContent sheenColorText = new GUIContent("Sheen Color", "Cloth ...");
            
            public static GUIContent subsurfaceColorText = new GUIContent("Subsurface Color", "Cloth");

            public static GUIContent specularMapText =
                new GUIContent("Specular Map", "Sets and configures the map and color for the Specular workflow.");

            public static GUIContent metallicMapText =
                new GUIContent("Metallic Map", "Sets and configures the map for the Metallic workflow.");

            public static GUIContent smoothnessText = new GUIContent("Smoothness",
                "Controls the spread of highlights and reflections on the surface.");

            public static GUIContent smoothnessMapChannelText =
                new GUIContent("Source",
                    "Specifies where to sample a smoothness map from. By default, uses the alpha channel for your map.");

            public static GUIContent highlightsText = new GUIContent("Specular Highlights",
                "When enabled, the Material reflects the shine from direct lighting.");

            public static GUIContent reflectionsText =
                new GUIContent("Environment Reflections",
                    "When enabled, the Material samples reflections from the nearest Reflection Probes or Lighting Probe.");

            public static GUIContent occlusionText = new GUIContent("Occlusion Map",
                "Sets an occlusion map to simulate shadowing from ambient lighting.");

            public static readonly string[] metallicSmoothnessChannelNames = {"Metallic Alpha", "Albedo Alpha"};
            public static readonly string[] specularSmoothnessChannelNames = {"Specular Alpha", "Albedo Alpha"};
        }

        public struct LitProperties
        {
            // Surface Option Props
            public MaterialProperty workflowMode;
            
            public MaterialProperty shadingModel;
            // Surface Input Props
            public MaterialProperty metallic;
            public MaterialProperty specColor;
            public MaterialProperty metallicGlossMap;
            public MaterialProperty specGlossMap;
            public MaterialProperty smoothness;
            public MaterialProperty smoothnessMapChannel;
            public MaterialProperty bumpMapProp;
            public MaterialProperty bumpScaleProp;
            public MaterialProperty occlusionStrength;
            public MaterialProperty occlusionMap;
            
            // Clear Coat
            public MaterialProperty clearCoat;
            public MaterialProperty clearCoatRoughness;
            
            // Cloth
            public MaterialProperty sheenColor;
            public MaterialProperty subsurfaceColor;

            // Advanced Props
            public MaterialProperty highlights;
            public MaterialProperty reflections;

            public LitProperties(MaterialProperty[] properties)
            {
                // Surface Option Props
                workflowMode = BaseShaderGUI.FindProperty("_WorkflowMode", properties, false);
                shadingModel = BaseShaderGUI.FindProperty("_ShadingModelID", properties, false);
                
                // Clear Coat
                clearCoat = BaseShaderGUI.FindProperty("_ClearCoat", properties, false);
                clearCoatRoughness = BaseShaderGUI.FindProperty("_ClearCoatRoughness", properties, false);
                
                // Cloth
                sheenColor = BaseShaderGUI.FindProperty("_SheenColor", properties, false);
                subsurfaceColor = BaseShaderGUI.FindProperty("_SubsurfaceColor", properties, false);
                
                // Surface Input Props
                metallic = BaseShaderGUI.FindProperty("_Metallic", properties);
                specColor = BaseShaderGUI.FindProperty("_SpecColor", properties, false);
                metallicGlossMap = BaseShaderGUI.FindProperty("_MetallicGlossMap", properties);
                specGlossMap = BaseShaderGUI.FindProperty("_SpecGlossMap", properties, false);
                smoothness = BaseShaderGUI.FindProperty("_Smoothness", properties, false);
                smoothnessMapChannel = BaseShaderGUI.FindProperty("_SmoothnessTextureChannel", properties, false);
                bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
                bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);
                occlusionStrength = BaseShaderGUI.FindProperty("_OcclusionStrength", properties, false);
                occlusionMap = BaseShaderGUI.FindProperty("_OcclusionMap", properties, false);
                // Advanced Props
                highlights = BaseShaderGUI.FindProperty("_SpecularHighlights", properties, false);
                reflections = BaseShaderGUI.FindProperty("_EnvironmentReflections", properties, false);
            }
        }

        public static void Inputs(LitProperties properties, MaterialEditor materialEditor, Material material)
        {
            DoMetallicSpecularArea(properties, materialEditor, material);
            BaseShaderGUI.DrawNormalArea(materialEditor, properties.bumpMapProp, properties.bumpScaleProp);

            if (properties.occlusionMap != null)
            {
                materialEditor.TexturePropertySingleLine(Styles.occlusionText, properties.occlusionMap,
                    properties.occlusionMap.textureValue != null ? properties.occlusionStrength : null);
            }

            if (properties.clearCoat != null && properties.clearCoatRoughness != null)
            {
                // 需要判断是否点选了ClearCoat模式，只在此模式显示
                var isClearCoat = ((BaseShaderGUI.ShadindModel)material.GetFloat("_ShadingModelID") == BaseShaderGUI.ShadindModel.CleatCoat);

                if (isClearCoat)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    var clearCoatValue = EditorGUILayout.Slider(Styles.clearCoatText, properties.clearCoat.floatValue, 0f, 1f);
                    if (EditorGUI.EndChangeCheck())
                        properties.clearCoat.floatValue = clearCoatValue;
                    EditorGUI.indentLevel--;
                
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    var clearCoatRoughnessValue = EditorGUILayout.Slider(Styles.clearCoatRoughnessText, properties.clearCoatRoughness.floatValue, 0f, 1f);
                    if (EditorGUI.EndChangeCheck())
                        properties.clearCoatRoughness.floatValue = clearCoatRoughnessValue;
                    EditorGUI.indentLevel--;
                }
            }

            if (properties.sheenColor != null && properties.subsurfaceColor != null)
            {
                var isCloth = ((BaseShaderGUI.ShadindModel)material.GetFloat("_ShadingModelID") == BaseShaderGUI.ShadindModel.Cloth);

                if (isCloth)
                {
                    var rect = EditorGUILayout.GetControlRect();
                
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    var sheen = EditorGUI.ColorField(rect, Styles.sheenColorText, properties.sheenColor.colorValue, true, false, false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo(properties.sheenColor.displayName);
                        properties.sheenColor.colorValue = sheen;
                    }
                    EditorGUI.indentLevel--;
                    // rect.y += 100.0f;
                    
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    var subsurface = EditorGUI.ColorField(EditorGUILayout.GetControlRect(), Styles.subsurfaceColorText, properties.subsurfaceColor.colorValue, true, false, false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        materialEditor.RegisterPropertyChangeUndo(properties.subsurfaceColor.displayName);
                        properties.subsurfaceColor.colorValue = subsurface;
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        public static void DoMetallicSpecularArea(LitProperties properties, MaterialEditor materialEditor, Material material)
        {
            string[] smoothnessChannelNames;
            bool hasGlossMap = false;
            if (properties.workflowMode == null ||
                (WorkflowMode) properties.workflowMode.floatValue == WorkflowMode.Metallic)
            {
                hasGlossMap = properties.metallicGlossMap.textureValue != null;
                smoothnessChannelNames = Styles.metallicSmoothnessChannelNames;
                materialEditor.TexturePropertySingleLine(Styles.metallicMapText, properties.metallicGlossMap,
                    hasGlossMap ? null : properties.metallic);
            }
            else
            {
                hasGlossMap = properties.specGlossMap.textureValue != null;
                smoothnessChannelNames = Styles.specularSmoothnessChannelNames;
                BaseShaderGUI.TextureColorProps(materialEditor, Styles.specularMapText, properties.specGlossMap,
                    hasGlossMap ? null : properties.specColor);
            }
            EditorGUI.indentLevel++;
            DoSmoothness(properties, material, smoothnessChannelNames);
            EditorGUI.indentLevel--;
        }

        public static void DoSmoothness(LitProperties properties, Material material, string[] smoothnessChannelNames)
        {
            var opaque = ((BaseShaderGUI.SurfaceType) material.GetFloat("_Surface") ==
                          BaseShaderGUI.SurfaceType.Opaque);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = properties.smoothness.hasMixedValue;
            var smoothness = EditorGUILayout.Slider(Styles.smoothnessText, properties.smoothness.floatValue, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
                properties.smoothness.floatValue = smoothness;
            EditorGUI.showMixedValue = false;

            if (properties.smoothnessMapChannel != null) // smoothness channel
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!opaque);
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = properties.smoothnessMapChannel.hasMixedValue;
                var smoothnessSource = (int) properties.smoothnessMapChannel.floatValue;
                if (opaque)
                    smoothnessSource = EditorGUILayout.Popup(Styles.smoothnessMapChannelText, smoothnessSource,
                        smoothnessChannelNames);
                else
                    EditorGUILayout.Popup(Styles.smoothnessMapChannelText, 0, smoothnessChannelNames);
                if (EditorGUI.EndChangeCheck())
                    properties.smoothnessMapChannel.floatValue = smoothnessSource;
                EditorGUI.showMixedValue = false;
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        public static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
        {
            int ch = (int) material.GetFloat("_SmoothnessTextureChannel");
            if (ch == (int) SmoothnessMapChannel.AlbedoAlpha)
                return SmoothnessMapChannel.AlbedoAlpha;

            return SmoothnessMapChannel.SpecularMetallicAlpha;
        }

        public static void SetMaterialKeywords(Material material)
        {
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            var hasGlossMap = false;
            var isSpecularWorkFlow = false;
            var opaque = ((BaseShaderGUI.SurfaceType) material.GetFloat("_Surface") ==
                          BaseShaderGUI.SurfaceType.Opaque);
            if (material.HasProperty("_WorkflowMode"))
            {
                isSpecularWorkFlow = (WorkflowMode) material.GetFloat("_WorkflowMode") == WorkflowMode.Specular;
                if (isSpecularWorkFlow)
                    hasGlossMap = material.GetTexture("_SpecGlossMap") != null;
                else
                    hasGlossMap = material.GetTexture("_MetallicGlossMap") != null;
            }
            else
            {
                hasGlossMap = material.GetTexture("_MetallicGlossMap") != null;
            }

            if (material.HasProperty("_ShadingModelID"))
            {
                CoreUtils.SetKeyword(material, "_MATERIAL_SHADINGMODEL_CLEAR_COAT", (BaseShaderGUI.ShadindModel)material.GetFloat("_ShadingModelID") == BaseShaderGUI.ShadindModel.CleatCoat);
                CoreUtils.SetKeyword(material, "_MATERIAL_SHADINGMODEL_CLOTH", (BaseShaderGUI.ShadindModel)material.GetFloat("_ShadingModelID") == BaseShaderGUI.ShadindModel.Cloth);
            }
                
            CoreUtils.SetKeyword(material, "_SPECULAR_SETUP", isSpecularWorkFlow);

            CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasGlossMap);

            if (material.HasProperty("_SpecularHighlights"))
                CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF",
                    material.GetFloat("_SpecularHighlights") == 0.0f);
            if (material.HasProperty("_EnvironmentReflections"))
                CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF",
                    material.GetFloat("_EnvironmentReflections") == 0.0f);
            if (material.HasProperty("_OcclusionMap"))
                CoreUtils.SetKeyword(material, "_OCCLUSIONMAP", material.GetTexture("_OcclusionMap"));

            if (material.HasProperty("_SmoothnessTextureChannel"))
            {
                CoreUtils.SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
                    GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha && opaque);
            }
        }
    }
}
