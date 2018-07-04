﻿using System;
using System.Collections.Generic;
using TLMath;
using TLExtensions;

public partial class Timeline
{
    public partial class Core
    {
        public Binding binding = new Binding();

        public class Bind
        {
            public string binding;
            public string property;
            public float value;
        }
        public class Binding
        {
            Timeline _timeline;
            Core _code;
            Access _access;
            public float[] data;
            public int nodesPerStream;
            public int propsPerNode;
            public int highestPropsPerNode;
            //public int[] propsPerNodeList;
            public Dictionary<int, int> propsPerNodeList = new Dictionary<int, int>();
            public int propDataLength;
            public int continuancePosValData0;
            public int data0PropDataLength;
            public int nodeDataLength;
            public int streamDataLength;
            public object proxy = null;
            public string state;

            public void Init(Timeline timeline)
            {
                this._timeline = timeline;
                this._code = timeline.code;
                this._access = timeline.access;

                Test();
                TimelineCode.Log("Init Binding");
            }
            private int variable;
            public void Test()
            {
                
                
                /*TLVector3 transform = new TLVector3(10.0f, 2.0f, 3.0f, "translate");

                transform.timeline.x.At(100);// ToDo Work on chained methods
                
                Add(
                    new object[]{
                            this._timeline,
                            this._timeline,
                            this._timeline,
                            this._timeline,
                            transform, 101,
                            "x", 0f, 100f,
                            "y", 0f, 200f,
                            102,
                            false,
                            1F
                    }
                );
                
                
                float[] audioFreqData = new float[]{3f,2f,1F};
                TLPoly freq = new TLPoly();
                Add(
                    new object[]{
                            this._timeline,
                            this._timeline,
                            this._timeline,
                            this._timeline,
                            freq, 800,
                            "poly", audioFreqData, //(fills in or replace freq[] data, skips null)
                            802,
                            false,
                            1F
                    }
                );
                */
            }
            public object Agent()
            {
                // Fill in timeline with frame values/ establish duration data for playback timeframe helper and keyframes for systems (particles, grids, chains and physic) 
                var element = new { value = 0, type = "uniform" };

                return Add(new object[]{
                    _timeline,// TODO all timelines
                    element, 101,
                    "value", 0f, (float)_timeline.length,
                    102,
                    false,
                    1F
                });
            }
            public Dictionary<int, IDictionary<int, object>> ids = new Dictionary<int, IDictionary<int, object>>();
            int buffIdKey = 800;
            int propIdKey;
            public int increment = 0;
            public int dataIncrement = 0;
            public TLType[] Add(object[] options, object[] instructionsSet = null)
            {
                object[] instructions = null;
                if (instructionsSet != null)
                    instructions = instructionsSet; 
                else if (options != null)
                    instructions = _code.BindInstructionSet(options);
                else
                    return null;

                object[] timelines = (object[])instructions[0];
                int timelineCount = timelines.Count();

                object[] objectParams = (object[])instructions[1];
                int objCount = objectParams.Count();

                object[] paramKeys = (object[])instructions[3];

                object[] propValues = (object[])instructions[2];

                bool relative = (bool?)instructions[4] ?? _access.defaults.relative;

                TLType[] nodes = new TLType[objCount];
                
                for (int t = 0; t < timelineCount; t++)
                {
                    Timeline timeline = (Timeline)timelines[t];

                    Binding binding = (Binding)timeline.code.binding;

                    if (binding.proxy == null)
                    {
                        
                        binding.nodesPerStream = 0;
                        binding.propsPerNode = instructions[3] != null ? instructions[3].Count() : instructions[2] != null ? instructions[2].Count() : 0;
                        binding.propDataLength = timeline.length;
                        //binding.propsPerNodeList = new int[1000/*queue.propsPerNodeList*/];
                        binding.state = "prebuff";
                        binding.continuancePosValData0 = 1;
                        binding.data0PropDataLength = binding.continuancePosValData0 + binding.propDataLength;
                        binding.nodeDataLength = binding.data0PropDataLength * binding.propsPerNode + binding.propsPerNode + 1;
                        binding.streamDataLength = 0;
                        binding.data = new float[0];
                        _code.reversion = (int dataPos) =>
                        {
                            return dataPos - (binding.propDataLength * (dataPos / binding.propDataLength << 0)) + binding.continuancePosValData0;
                        };
                        binding.proxy = 1;
                        binding.proxy = Agent();
                    }
                    

                    //int[] runtimePropsPerNodeList = binding.propsPerNodeList;
                    // comment out for dynamic keys
                    //int runtimeLastPropsPerNode = binding.propsPerNodeList.Keys.Max() - 1;
                    binding.nodesPerStream += instructions[1].Count();
                    float[] predata = null;

                    for (int o = 0; o < objCount; o++)
                    {
                        object[] obj = (object[])objectParams[o];
                        var param = obj[0] as TLType;
                        if (obj[0] is TLPoly)
                            param = obj[0] as TLPoly;
                        else if (obj[0] is TLVector3)
                            param = obj[0] as TLVector3;
                        else if (obj[0] is TLElement)
                            param = obj[0] as TLElement;

                        // TODO refactor - move to class declarations
                        string dataType = (string)_code.CheckListGet(param.type, new
                                                string[]{
                                                    "position=translation",
                                                    "Vector4=translation",
                                                    "Vector3=translation",
                                                    "Vector2=translation",
                                                    "rotation=radian",
                                                    "Euler=radian",
                                                    "translate=translation",
                                                    "translation="+param.type,
                                                    "radian="+param.type,
                                                    "poly="+param.type,
                                                    "other=uniform"
                                                });
                        // no precision conversion nessary already float points
                        //float dataTypePrecision = (float?)instructions[5] ?? (float)TMath.Type.Precision(dataType);

                        int? paramKey = (int?)paramKeys[o];

                        object[] initProp = (object[])propValues[0];
                        //string propName = (string)initProp[0];
                        object initPropValue = (object)initProp[1];
                        float? propValueEnd = (float?)initProp[2];
                        float? propPrecision = (float?)initProp[3];

                        object[] props;
                        object[] pkeys;

                        propIdKey = dataType == "radian" ? 806 : 801;
                        if (param is TLPoly)
                        {
                            props = TMath.Poly.Generate(dataType, (float[])param.poly, (float[])initPropValue);
                            //props = TMath.Poly.Generate(dataType, (float[])param.poly, (float[])initPropValue, dataTypePrecision);
                            pkeys = props.Length == paramKeys.Count() ? paramKeys : TMath.Poly.GenerateKeys(props, paramKey ?? propIdKey);
                        }
                        else
                        {
                            props = TMath.Type.ConvertToTypeData(dataType, (object[])propValues, 1, 2);
                            //props = TMath.Type.convertToPrecisionDataType(dataType, (object[])propValues, 1, 2, dataTypePrecision);
                            pkeys = props.Length == paramKeys.Count() ? paramKeys : TMath.Poly.GenerateKeys(props, paramKey ?? propIdKey);
                        }

                        binding.propsPerNode = props.Length;
                        binding.highestPropsPerNode = binding.propsPerNode > binding.highestPropsPerNode ? binding.propsPerNode : binding.highestPropsPerNode;
                        
                        binding.nodeDataLength = binding.data0PropDataLength * binding.propsPerNode + binding.propsPerNode + 1;

                        binding.streamDataLength += binding.nodeDataLength;
                        float[] zeroin = new float[binding.nodeDataLength];
                        if (predata == null) {
                            binding.increment = 0;
                            predata = new float[0];
                        }
                        predata = predata.Concat(zeroin);

                        int buffKey = (int?)obj[1] ?? binding.buffIdKey;

                        IDictionary<int, object> buffKeyDic;
                        if (binding.ids.TryGetValue(buffKey, out buffKeyDic))
                        {
                            /* Cannot overwrite */
                        }
                        else
                        {
                            binding.ids.Add(buffKey, new Dictionary<int, object>() { { 0, param } });
                        }

                        predata[binding.increment++] = (binding.buffIdKey = buffKey);           
                                binding.dataIncrement++;// For the node slot, assign buffIdKey for ensure consistency & no conflict
                        binding.buffIdKey++; // increment
                        // comment out for dynamic keys
                        // if (runtimePropsPerNodeList[runtimeLastPropsPerNode] != pkeys.length)
                        //binding.propsPerNodeList.Add(buffKey, binding.propsPerNode);
                        int propsPerNodeListDic;
                        if (binding.propsPerNodeList.TryGetValue(buffKey, out propsPerNodeListDic))
                        {
                            /* Cannot overwrite */
                            //binding.propsPerNodeList[buffKey] = binding.propsPerNode;
                        }
                        else
                        {
                            binding.propsPerNodeList.Add(buffKey, binding.propsPerNode);
                        }
                        
                        for (int p = 0; p < props.Length; p++)
                        {
                            object[] prop = (object[])props[p];
                            string propName = (string)prop[0];
                            float propValue = (float?)prop[1] ?? 0;
                            bool hasEndValue = prop[2] != null;
                            float endValue = hasEndValue ? (float)prop[2] : 0;

                            int propKey = pkeys[p] != null ? (int)pkeys[p] : binding.propIdKey;

                            predata[binding.increment++] = propKey;
                                    binding.dataIncrement++;
                            predata[binding.increment++] = 1;
                                    binding.dataIncrement++;

                            var properties = (ExecParams)param.GetMember(timeline.stream + "." + propName);
                            properties.binding = buffKey;
                            properties.position = binding.dataIncrement;
                            properties.relative = relative;
                            properties.conversion = dataType;
                            binding.dataIncrement += propDataLength;
                            //properties.precision = dataTypePrecision;

                            bool isDomElement = dataType != "poly" && param.GetMember(propName + ".value") != null;
                            // TO-DO rework binding for all or future demos, uniform scheme
                            // Assign starting value to both stream value and node property 

                            object propKeyObject;
                            if (binding.ids[buffKey].TryGetValue(propKey, out propKeyObject)) 
                            {
                                /* Cannot overwrite
                                binding.ids[buffKey][propKey] = new
                                {
                                    binding = isDomElement ? "value" : propName,
                                    property = isDomElement ? propName : null,
                                    value = propValue
                                };
                                */
                            }
                            else
                            {
                                binding.ids[buffKey].Add(propKey, new Bind
                                {
                                    binding = isDomElement ? "value" : propName,
                                    property = isDomElement ? propName : null,
                                    value = propValue
                                });
                            }

                            if (isDomElement)
                            {
                                param.GetMember(propName + ".value",
                                propValue
                                //propName == "value" ? propValue : propValue / dataTypePrecision
                                );
                            }
                            else
                            {
                                param.GetMember(propName,
                                propValue
                                //propName == "value" ? propValue : propValue / dataTypePrecision
                                );
                            }

                            for (int bDI = 0, alen = binding.propDataLength; bDI < alen; bDI++) {
                                if (hasEndValue) {
                                    if (relative) {
                                        predata[binding.increment++] = ((endValue - propValue) / propDataLength);
                                    } else {
                                        predata[binding.increment++] = (propValue + ((endValue - propValue) / propDataLength * bDI));
                                    }
                                } else {
                                    if (relative) {
                                        predata[binding.increment++] = 0;
                                    } else {
                                        predata[binding.increment++] = propValue;
                                    }
                                }
                            }
                            binding.propIdKey = propKey;
                            binding.propIdKey++;
                        }
                        nodes[o] = param;
                    }
                    binding.data = binding.data.Concat(predata);
                }
                return nodes;
            }
            public Dictionary<int, IDictionary<int, object[]>> list = new Dictionary<int, IDictionary<int, object[]>>();
            public object Queue(object[] options, object[] instructionsSet = null)
            {
                object[] instructions = null;
                if (instructionsSet != null)
                    instructions = instructionsSet; 
                else if (options != null)
                    instructions = _code.BindInstructionSet(options);
                else
                    return 0;

                object[] timelines = (object[])instructions[0];
                int timelineCount = timelines.Count();

                object[] objectParams = (object[])instructions[1];
                int objCount = objectParams.Count();

                object[] paramKeys = (object[])instructions[3];

                object[] propValues = (object[])instructions[2];

                bool relative = (bool?)instructions[4] ?? _access.defaults.relative;

                float dataTypePrecision = 1;

                object[] nodes = new object[objCount];

                int oi = 0;

                int propsPerNode = objCount;

                for (int t = 0; t < timelineCount; t++)
                {
                    Timeline timeline = (Timeline)timelines[t];

                    Binding binding = (Binding)timeline.code.binding;

                    while (objCount > oi)
                    {
                        object[] obj = (object[])objectParams[oi];
                        var param = obj[0] as TLType;
                        if (obj[0] is TLPoly)
                            param = obj[0] as TLPoly;
                        else if (obj[0] is TLVector3)
                            param = obj[0] as TLVector3;
                        else if (obj[0] is TLElement)
                            param = obj[0] as TLElement;

                        // TODO refactor - move to class declarations
                        string dataType = (string)_code.CheckListGet(param.type, new
                        string[]{
                        "position=translation",
                        "Vector4=translation",
                        "Vector3=translation",
                        "Vector2=translation",
                        "rotation=radian",
                        "Euler=radian",
                        "translate=translation",
                        "translation="+param.type,
                        "radian="+param.type,
                        "poly="+param.type,
                        "other=uniform"
                        });

                        int? paramKey = (int?)paramKeys[oi];

                        object[] initProp = (object[])propValues[0];
                        //string propName = (string)initProp[0];
                        object initPropValue = (object)initProp[1];
                        float? propValueEnd = (float?)initProp[2];
                        float? propPrecision = (float?)initProp[3];

                        object[] props;
                        object[] pkeys;

                        propIdKey = dataType == "radian" ? 806 : 801;
                        if (param is TLPoly)
                        {
                            // TODO test poly in queue, will it duplicate proccess
                            props = TMath.Poly.Generate(dataType, (float[])param.poly, (float[])initPropValue, dataTypePrecision);
                        }
                        else
                        {
                            props = propValues;
                        }

                        propsPerNode = props.Length;

                        int pi = 0;
                        if (dataType != "poly")
                            while (props.Length > pi)
                            {

                                object[] prop = (object[])props[pi];
                                string propName = (string)prop[0];
                                float propValue = (float?)prop[1] ?? 0;

                                bool isDomElement = param.GetMember(propName + ".value") != null;

                                if (isDomElement)
                                {
                                    param.GetMember(propName + ".value", propValue);
                                }
                                else
                                {
                                    param.GetMember(propName, propValue);
                                }

                                pi++;
                            }

                        nodes[oi] = param;
                        oi++;
                    }
                    IDictionary<int, object[]> propsPerNodeDic;
                    if (binding.list.TryGetValue(propsPerNode, out propsPerNodeDic))
                    {
                        binding.list[propsPerNode].Add(binding.list[propsPerNode].Count, instructions);
                    }
                    else
                    {
                        binding.list.Add(propsPerNode, new Dictionary<int, object[]>() { { 0, instructions } });
                    }
                }

                return nodes;
            }
            public void Build(Func<int> CallBack = null) {
                Msg(Scenes.timeline);
                BuildOff(Scenes.timeline);
                if (CallBack != null) CallBack();
            }
            public void Build(Timeline timeline, Func<int> CallBack = null) {
                Msg(timeline);
                BuildOff(timeline);
                if (CallBack != null) CallBack();
            }
            public void Build(Timeline[] timelines, Func<int> CallBack = null) {
                for (int t = 0; t < timelines.Length; t++) {
                    Msg(timelines[t]);
                    BuildOff(timelines[t]);
                }
                if (CallBack != null) CallBack();
            }
            void Msg(Timeline timeline) {
                TimelineCode.Log("("+timeline.name+")"+" Binding objects to stream - Running queue");
            }
            void BuildOff(Timeline timeline = null) {
                Binding binding;
                binding = timeline != null? timeline.code.binding : _code.binding;

                foreach(KeyValuePair<int, IDictionary<int, object[]>> buffKeyDic in this.list)
                {
                    foreach(KeyValuePair<int, object[]> propsPerNodeDic in buffKeyDic.Value)
                    {
                        this.Add(null, propsPerNodeDic.Value);
                    }
                }
                this.list.Clear();
            }
        }
    }
}

