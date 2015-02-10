using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows.Shapes;
using Common.Controls;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Preview.Vixen3DPreview
{
    class PreviewUtils
    {
        private XMLProfileSettings xmlSettings = new XMLProfileSettings();

        /// <summary>
        /// Put bhe vertices for the lines of a wireframe cube into an array.
        /// This allows us to use GL.Vertex3 or the VAO
        /// </summary>
        /// <param name="x">Lower left corner x</param>
        /// <param name="y">Lower left corner y</param>
        /// <param name="z">Lower left corner z</param>
        /// <param name="width">Cube Width</param>
        /// <param name="height">Cube Height</param>
        /// <param name="depth">Cube Depth</param>
        /// <returns></returns>
        public Vector3[] GetCubeVertexes(float x, float y, float z,
            float width, float height, float depth)
        {
            var cubeIndices = new int[]
            {
                // front face
                0, 1, 1, 2, 2, 3, 3, 0,
                // back face
                4, 5, 5, 6, 6, 7, 7, 4,
                // left face
                0, 4, 3, 7,
                // right face
                1, 5, 2, 6
            };

            float zd = z + depth;
            float xw = x + width;
            float yh = y + height;

            var cubeVertices = new Vector3[]
            {
                new Vector3(x, y, z),
                new Vector3(xw, y, z),
                new Vector3(xw, yh, z),
                new Vector3(x, yh, z),
                new Vector3(x, y, -zd),
                new Vector3(xw, y, -zd),
                new Vector3(xw, yh, -zd),
                new Vector3(x, yh, -zd)
            };

            var lineVertices = new Vector3[cubeIndices.Length];
            var vertexNum = 0;
            foreach (var index in cubeIndices)
            {
                lineVertices[vertexNum] = cubeVertices[index];
                vertexNum++;
            }
            return lineVertices;
        }

        public void SavePrivateSetting(string name, string value)
        {
            xmlSettings.PutSetting(XMLProfileSettings.SettingType.Preview, name, value);
        }

        public void SavePrivateSetting(string name, int value)
        {
            xmlSettings.PutSetting(XMLProfileSettings.SettingType.Preview, name, value);
        }

        public void SavePrivateSetting(string name, bool value)
        {
            xmlSettings.PutSetting(XMLProfileSettings.SettingType.Preview, name, value);
        }

        public string GetPrivateSetting(string name, string defaultValue)
        {
            return xmlSettings.GetSetting(XMLProfileSettings.SettingType.Preview, name, defaultValue);
        }

        public int GetPrivateSetting(string name, int defaultValue)
        {
            return xmlSettings.GetSetting(XMLProfileSettings.SettingType.Preview, name, defaultValue);
        }

        public bool GetPrivateSetting(string name, bool defaultValue)
        {
            return xmlSettings.GetSetting(XMLProfileSettings.SettingType.Preview, name, defaultValue);
        }

        public static void SaveData(Vixen3DPreviewPrivateData data)
        {
            var s = new DataContractSerializer(typeof(Vixen3DPreviewPrivateData));
            var fileName = ModuleFilename(data.InstanceId + ".xml");
            //Console.WriteLine(fileName);
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            {
                //Console.WriteLine("Testing for type: {0}", typeof(T));
                s.WriteObject(fs, data);
            }
        }

        public static Vixen3DPreviewPrivateData ReadData(string instanceId)
        {
            Vixen3DPreviewPrivateData data;
            var s = new DataContractSerializer(typeof(Vixen3DPreviewPrivateData));
            object deserializedObject = null;
            var fileName = ModuleFilename(instanceId + ".xml");
            if (File.Exists(fileName))
            {
                //Console.WriteLine("File exists: " + fileName);
                using (FileStream fs = File.Open(fileName, FileMode.Open))
                {
                    deserializedObject = s.ReadObject(fs);
                    if (deserializedObject == null)
                    {
                        Console.WriteLine("Unable to deserialize object");
                        deserializedObject = new Vixen3DPreviewPrivateData(instanceId);
                    }
                }
            }
            else
            {
                deserializedObject = new Vixen3DPreviewPrivateData(instanceId);
            }
            var previewData = (deserializedObject as Vixen3DPreviewPrivateData);
            previewData.InstanceId = instanceId;
            return (deserializedObject as Vixen3DPreviewPrivateData);
        }

        public static string ModuleFilename(string fileName)
        {
            return System.IO.Path.Combine(Vixen3DPreviewDescriptor.ModulePath, fileName);
        }

        /// <summary>
        /// Returns the child nodes that do not have children
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static List<ElementNode> GetLeafNodes(ElementNode node)
        {
            return node.Children.Where(child => child.IsLeaf).ToList();
        }

        public static List<ElementNode> GetParentNodes(ElementNode node)
        {
            return node.Children.Where(child => !child.IsLeaf).ToList();
        }
    }
}
