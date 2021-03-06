﻿using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Object = UnityEngine.Object;
/*
 * gameLoadType 为 Resource 时 ，所有资源从Resource读取
 * gameLoadType 不为 Resource时，资源读取方式从配置中读取
 * */
public static class ResourceManager 
{
    private static AssetsLoadType loadType = AssetsLoadType.Resources;
    public static AssetsLoadType LoadType
    {
        get
        {
            return loadType;
        }

        set
        {
            ReleaseAll();

            loadType = value;
            isInit = false;
            Initialize();
        }
    }

    private static AssetsLoadController loadAssetsController;

    private static bool isInit = false;
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
#endif
    private static void Initialize()
    {
        if (isInit)
            return;
        loadAssetsController = new AssetsLoadController(loadType);
    }

    public static AssetsLoadController GetLoadAssetsController()
    {
        return loadAssetsController;
    }

    /// <summary>
    /// 同步加载一个资源
    /// </summary>
    /// <param name="name"></param>
    public static Object Load(string name)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        AssetsData assets = loadAssetsController.LoadAssets(path);
        if (assets != null)
        {
            return assets.Assets[0];
        }
        return null;
    }

    public static void LoadAsync(string name, CallBack<Object> callBack)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        loadAssetsController.LoadAsync(path, null, callBack);
    }
    public static void LoadAsync(string name, Type resType, CallBack<Object> callBack)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        loadAssetsController.LoadAsync(path, resType, callBack);
    }
    public static T Load<T>(string name) where T : Object
    {
        T res =null;
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        //Debug.Log("ResourcesConfigManager.GetLoadPath :"+ path);
        AssetsData assets = loadAssetsController.LoadAssets<T>(path);
        if (assets != null)
        {
            res = assets.GetAssets<T>();

        }
       if(res ==null)
        {
            Debug.LogError("Error=> Load Name :" + name + "  Type:" + typeof(T).FullName + "\n" + " Load Object:" + res );
        }
        return res;
    }
    public static T EditorLoad<T>(string name) where T : Object
    {
        T res = null;
        string path = ResourcesConfigManager.GetLoadPath( AssetsLoadType.Resources, name);
        res = Resources.Load<T>(path);
        return res;
    }
    public static string LoadText(string name)
    {
        TextAsset tex = Load<TextAsset>(name);
        if (tex == null)
            return null;
        return tex.text;
    }

    public static void DestoryAssetsCounter(Object unityObject, int times = 1)
    {
        DestoryAssetsCounter(unityObject.name, times);
    }

    public static void DestoryAssetsCounter(string name, int times = 1)
    {
        if (!ResourcesConfigManager.GetIsExitRes(name))
            return;
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        if (times <= 0)
            times = 1;
        for (int i = 0; i < times; i++)
        {
            loadAssetsController.DestoryAssetsCounter(path);
        }
    }

    /// <summary>
    /// 卸载所有资源
    /// </summary>
    /// <param name="isForceAB">是否强制卸载bundle</param>
    public static void ReleaseAll(bool isForceAB=true)
    {
        if (loadAssetsController != null)
            loadAssetsController.ReleaseAll(isForceAB);
        //ResourcesConfigManager.ClearConfig();
    }

    public static void Release(string name)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        loadAssetsController.Release(path);
    }

    public static void ReleaseByPath(string path)
    {
        loadAssetsController.Release(path);
    }

    public static bool GetResourceIsExist(string name)
    {
        return ResourcesConfigManager.GetIsExitRes(name);
    }
}





