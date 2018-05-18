using UnityEngine;

namespace math
{
    public partial class Math
    {
        public static class Poly
        {
            public static object[] poly(string type, float[] data, float precision)
            {
                object[][] poly = new object[data.Length][];
                for (int di = 0, dlen = data.Length; di < dlen; di++)
                {
                    poly[di] = new object[]
                    {
                        di,
                        Type.convertToPrecisionType(type, data[di], precision)
                    };
                }
                return poly;
            }

            public static object[] generate(string type, float[] data, float precision)
            {
                object[][] poly = new object[data.Length][];
                for (int di = 0, dlen = data.Length; di < dlen; di++)
                {
                    poly[di] = new object[]
                    {
                        di,
                        Type.convertToPrecisionType(type, data[di], precision)
                    };
                }
                return poly;
            }

            public static int[] generateKeys(float[] data, int? start)
            {
                int[] poly = new int[data.Length];
                int _start = start ?? 0;
                for (int di = 0, dlen = data.Length; di < dlen; di++)
                {
                    poly[di] = _start + di;
                }
                return poly;
            }

             public static object[] generateExec(string type, float[] data, float precision)
            {
                object[][] poly = new object[data.Length][];
                for (int di = 0, dlen = data.Length; di < dlen; di++)
                {
                    poly[di] = new object[]
                    {
                        di,
                        Type.convertToPrecisionType(type, data[di], precision)
                    };
                }
                return poly;
            }

            public static float[] add(float[] data1, float[] data2)
            {
                float[] poly = new float[data1.Length];
                for (int pi = 0, dlen = data1.Length; pi < dlen; pi++)
                {
                    poly[pi] = data1[pi] + data2[pi];
                }
                return poly;
            }

            public static float[] subtract(float[] data1, float[] data2)
            {
                float[] poly = new float[data1.Length];
                for (int pi = 0, dlen = data1.Length; pi < dlen; pi++)
                {
                    poly[pi] = data1[pi] - data2[pi];
                }
                return poly;
            }

            public static float[] multiply(float[] data1, float[] data2)
            {
                float[] poly = new float[data1.Length];
                for (int pi = 0, dlen = data1.Length; pi < dlen; pi++)
                {
                    poly[pi] = data1[pi] * data2[pi];
                }
                return poly;
            }

            public static float[] divide(float[] data1, float[] data2)
            {
                float[] poly = new float[data1.Length];
                for (int pi = 0, dlen = data1.Length; pi < dlen; pi++)
                {
                    poly[pi] = data1[pi] / data2[pi];
                }
                return poly;
            }

            public static float[] addScalar(float[] data, float val)
            {
                float[] poly = new float[data.Length];
                for (int pi = 0, dlen = data.Length; pi < dlen; pi++)
                {
                    poly[pi] = data[pi] + val;
                }
                return poly;
            }

            public static float[] subtractScalar(float[] data, float val)
            {
                float[] poly = new float[data.Length];
                for (int pi = 0, dlen = data.Length; pi < dlen; pi++)
                {
                    poly[pi] = data[pi] - val;
                }
                return poly;
            }


            public static float[] multiplyScalar(float[] data, float val)
            {
                float[] poly = new float[data.Length];
                for (int pi = 0, dlen = data.Length; pi < dlen; pi++)
                {
                    poly[pi] = data[pi] * val;
                }
                return poly;
            }

            public static float[] divideScalar(float[] data, float val)
            {
                float[] poly = new float[data.Length];
                for (int pi = 0, dlen = data.Length; pi < dlen; pi++)
                {
                    poly[pi] = data[pi] / val;
                }
                return poly;
            }

            public static void addScalarVector(float[] data, Vector2 v)
            {
                scalarVector("+=", data, new float?[2] { v.x, v.y });
            }

            public static void addScalarVector(float[] data, Vector3 v)
            {
                scalarVector("+=", data, new float?[3] { v.x, v.y, v.z });
            }

            public static void addScalarVector(float[] data, Vector4 v)
            {
                scalarVector("+=", data, new float?[4] { v.x, v.y, v.z, v.w });
            }

            public static void subtractScalarVector(float[] data, Vector2 v)
            {
                scalarVector("-=", data, new float?[2] { v.x, v.y });
            }

            public static void subtractScalarVector(float[] data, Vector3 v)
            {
                scalarVector("-=", data, new float?[3] { v.x, v.y, v.z });
            }

            public static void subtractScalarVector(float[] data, Vector4 v)
            {
                scalarVector("-=", data, new float?[4] { v.x, v.y, v.z, v.w });
            }

            public static void multiplyScalarVector(float[] data, Vector2 v)
            {
                scalarVector("*=", data, new float?[2] { v.x, v.y });
            }

            public static void multiplyScalarVector(float[] data, Vector3 v)
            {
                scalarVector("*=", data, new float?[3] { v.x, v.y, v.z });
            }

            public static void multiplyScalarVector(float[] data, Vector4 v)
            {
                scalarVector("*=", data, new float?[4] { v.x, v.y, v.z, v.w });
            }

            public static void divideScalarVector(float[] data, Vector2 v)
            {
                scalarVector("/=", data, new float?[2] { v.x, v.y });
            }

            public static void divideScalarVector(float[] data, Vector3 v)
            {
                scalarVector("/=", data, new float?[3] { v.x, v.y, v.z });
            }

            public static void divideScalarVector(float[] data, Vector4 v)
            {
                scalarVector("/=", data, new float?[4] { v.x, v.y, v.z, v.w });
            }

            public static void scalarVector(string operate, float[] data, float?[] poly)
            {
                // Add
                if (operate == "+=")
                    for (
                        int pi = 0, dlen = data.Length, plen = poly.Length, qual = 0;
                        pi < dlen;
                        pi++, qual = pi - (plen * (pi / plen << 0))
                    )
                        data[pi] += poly[qual] ?? 0F;

                // Subtract
                if (operate == "-=")
                    for (
                        int pi = 0, dlen = data.Length, plen = poly.Length, qual = 0;
                        pi < dlen;
                        pi++, qual = pi - (plen * (pi / plen << 0))
                    )
                        data[pi] -= poly[qual] ?? 0F;

                // Multiply
                if (operate == "*=")
                    for (
                        int pi = 0, dlen = data.Length, plen = poly.Length, qual = 0;
                        pi < dlen;
                        pi++, qual = pi - (plen * (pi / plen << 0))
                    )
                        data[pi] *= poly[qual] ?? 1F;

                //Divide
                if (operate == "/=")
                    for (
                        int pi = 0, dlen = data.Length, plen = poly.Length, qual = 0;
                        pi < dlen;
                        pi++, qual = pi - (plen * (pi / plen << 0))
                    )
                        data[pi] /= poly[qual] ?? 1F;
            }

            public static float[] smoothOut(float[] data, float variance)
            {
                float tAvg = averageArray(data) * variance;
                float[] poly = new float[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    float prev = i > 0 ? poly[i - 1] : data[i];
                    float next = i < data.Length ? data[i] : data[i - 1];
                    poly[i] = averageArray(new float[] { tAvg, averageArray(new float[] { prev, data[i], next }) });
                }
                return poly;
            }

            public static float sumArray(float[] array)
            {
                float result = 0;

                for (int i = 0; i < array.Length; i++)
                {
                    result += array[i];
                }

                return result;
            }

            public static float averageArray(float[] array)
            {
                float sumup = sumArray(array);
                float result = (float)sumup / array.Length;
                return result;
            }
            // var test = [[1, 2, 3, [1, 2, 3, [1, 2, 3], [11, 12, 13], [21, 22, 23]], [11, 12, 13], [21, 22, 23]], [11, 12, 13], [21, 22, 23]]
            // console.log(JSON.stringify(public static object[] generate('poly', test, 10)))

        }
    }
}