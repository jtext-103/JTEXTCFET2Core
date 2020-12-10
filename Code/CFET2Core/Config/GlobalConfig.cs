using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jtext103.CFET2.Core.Config
{
    public static class GlobalConfig
    {
        public static string IP { get; set; }
        public static string Port { get; set; }
        public static string Accept { get; set; }
        public static Uri HostUri
        {
            get
            {
                if (IP != null && Port != null)
                {
                    return new Uri(IP + ":" + Port);
                }
                else return null;    
            }
        }

        public static void Populate(string path)
        {
            var configDict = new Dictionary<string, string>();
            JsonConvert.PopulateObject(File.ReadAllText(path, Encoding.Default), configDict);
            try
            {
                IP = configDict["IP"];
                Port = configDict["Port"];
                Accept = configDict["Accept"];
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
        public static Dictionary<string,string> Get()
        {
            var configDict = new Dictionary<string, string>();
            configDict.Add("IP", IP);
            configDict.Add("Port", Port);
            configDict.Add("Accept", Accept);
            return configDict;
        }

        public static void Set(Dictionary<string, string> configDict)
        {
            try
            {
                IP = configDict["IP"];
                Port = configDict["Port"];
                Accept = configDict["Accept"];
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
    }
}
