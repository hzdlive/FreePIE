﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreePIE.Core.Contracts;
using FreePIE.Core.Plugins.OculusVR;

namespace FreePIE.Core.Plugins
{
    [GlobalType(Type = typeof(OculusGlobal))]
    public class OculusPlugin : Plugin
    {
        private float sensorPrediction;

        public override object CreateGlobal()
        {
            return new OculusGlobal(this);
        }

        public override string FriendlyName
        {
            get { return "Oculus VR"; }
        }

        public override bool GetProperty(int index, IPluginProperty property)
        {
            if (index > 0) return false;

            property.Name = "SensorPrediction";
            property.Caption = "Sensor prediction";
            property.HelpText = "Sensor prediction in seconds, 0.04 is a good start to test with";
            property.DefaultValue =  0f;

            return true;
        }

        public override bool SetProperties(Dictionary<string, object> properties)
        {
            sensorPrediction = (float) properties["SensorPrediction"];

            return true;
        }

        public override Action Start()
        {
            if (!Api.Init(sensorPrediction))
                throw new Exception("Oculus VR SDK failed to init");

            return null;
        }

        public override void Stop()
        {
            Api.Dispose();
        }

        public override void DoBeforeNextExecute()
        {
            Data = Api.Read();
            OnUpdate();
        }

        public void Center()
        {
            Api.Center();
        }

        public OculusVr3Dof Data
        {
            get; private set;
        }
    }

    [Global(Name = "oculusVR")]
    public class OculusGlobal : UpdateblePluginGlobal<OculusPlugin>
    {
        public OculusGlobal(OculusPlugin plugin) : base(plugin){}

        public float yaw { get { return plugin.Data.Yaw; } }
        public float pitch { get { return plugin.Data.Pitch; } }
        public float roll { get { return plugin.Data.Roll; } }

        public void center()
        {
            plugin.Center();
        }
    }
}
