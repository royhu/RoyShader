/********************************************************
 * Copyright(C) 2019 by RoyApp All rights reserved.
 * FileName:    AutoAddFileHead.cs
 * Author:      Roy Hu
 * Version:     1.0
 * Date:        2019-08-15 11:11
 * Description: Replacing the "Macro" in the script templates which locates in 
 *				"/Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates" (mac app)
 *******************************************************/

using System.IO;

namespace RoyUnity.Editor
{
	public class AutoAddFileHead : UnityEditor.AssetModificationProcessor
	{
		public static void OnWillCreateAsset(string fileMeta)
		{
			if (fileMeta.Contains("StreamingAssets/"))
			{
				return;
			}

			string filePath = fileMeta.Replace(".meta", "");
        	
			string fileExt = Path.GetExtension(filePath);
        	if (fileExt != ".cs" && !filePath.EndsWith(".lua") )
        	{
        	    return;
        	}

        	string realPath = UnityEngine.Application.dataPath.Replace("Assets", "") + filePath;
			string orginContent = File.ReadAllText(realPath);
			var nameSplits = Path.GetFileNameWithoutExtension(filePath).Split('_');
			string scriptContent = orginContent
        		.Replace("#SCRIPTFULLNAME#", Path.GetFileName(filePath))
				.Replace("#SCRIPT_TAIL_NAME#", nameSplits[nameSplits.Length - 1])
        		.Replace("#COMPANY#", EngineConst.Company)
        		.Replace("#AUTHOR#", EngineConst.Author)
        		.Replace("#VERSION#", EngineConst.Version)
        		.Replace("#YEAR#", System.DateTime.Now.Year.ToString())
				.Replace("#NAMESPACE#", EngineConst.Namespace)
        		.Replace("#DATE#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

			if (filePath.EndsWith(".lua"))
			{
				string luaFileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath));
				scriptContent = scriptContent.Replace("#LUASCRIPTNAME#", luaFileName);
			}

			if (orginContent != scriptContent)
        	{
				// TODO-roy: DO NOT CHANGE SOURCE ENCODING
				File.WriteAllText(realPath, scriptContent);
			}
		}
	}
}