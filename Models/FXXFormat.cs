using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H3EK_FaceFX_Wrapper.Models
{
    struct s_raw_facial_animation_key
    {
        public float time;
        public float weight;
        public float slope_in;
        public float slope_out;
    };

    struct s_raw_facial_animation_curve
    {
        public int key_count; // this may be an int64?
        public int unknown;
        public s_raw_facial_animation_key[] curve_keys;
    };

    struct s_facial_animation_file
    {
        public int export_version;
        public int unknown;
        public int checksum;
        public float start_time;
        public float end_time;
        public float blend_in;
        public float blend_out;
        public int unknown1;
        public int unknown2;
        public int unknown3;
        public s_raw_facial_animation_curve[] facial_animation_curves;
    }

    class FXX_File
    {
        private s_facial_animation_file file = new s_facial_animation_file { facial_animation_curves = new s_raw_facial_animation_curve[34] };
        public FXX_File(string ltfPath)
        {
            // set header info
            file.export_version = 4;
            file.blend_in = 0.16f;
            file.blend_out = 0.22f;
            file.checksum = 0;
            // test these!
            //file.name_length = 32;
            //file.group_name = new char[32];
            //file.group_name[0] = 't'; file.group_name[1] = 'e'; file.group_name[2] = 'm'; file.group_name[3] = 'p';
            //file.anim_name = new char[32];
            //file.anim_name[0] = 't'; file.anim_name[1] = 'e'; file.anim_name[2] = 'm'; file.anim_name[3] = 'p';
            file.unknown1 = 4; // hack to align the curve block with tool's read code
            file.unknown2 = 0;
            file.unknown3 = 0;

            using (StreamWriter logWriter = File.CreateText("FaceFXWrapper.log"))
            using (StreamReader reader = new StreamReader(File.OpenRead(ltfPath), Encoding.UTF8))
            {
                // verify we're looking at a valid LTF file
                bool ltfIsValid = false;
                if (reader.ReadLine() == "// LTF file" && reader.ReadLine() == "LTF" && reader.ReadLine() == "" && reader.ReadLine() == "// Version" && reader.ReadLine() == "1.1")
                    ltfIsValid = true;
                if (!ltfIsValid)
                {
                    logWriter.WriteLine("The LTF file was invalid!");
                    return;
                }

                // skip to curve data
                while (reader.ReadLine() != "// Num Curves");
                long curveCount = Convert.ToInt32(reader.ReadLine());

                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                // read curve data
                float lowestTime = 0.0f;
                float highestTime = 0.0f;
                for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
                {
                    string curveName = reader.ReadLine().Replace("	", "");
                    ECurveType curveType;
                    if (!Enum.TryParse(curveName, out curveType))
                    {
                        logWriter.WriteLine("Unrecognised curve type!");
                        return;
                    }

                    reader.ReadLine();
                    reader.ReadLine();
                    int keyCount = Convert.ToInt32(reader.ReadLine());
                    reader.ReadLine();
                    reader.ReadLine();

                    int pTo = 0;
                    string keysString = "";
                    file.facial_animation_curves[(int)curveType].key_count = keyCount;
                    file.facial_animation_curves[(int)curveType].curve_keys = new s_raw_facial_animation_key[keyCount];
                    // read key data
                    for (int keyIndex = 0; keyIndex < keyCount; keyIndex++)
                    {
                        // the first set of keys are printed in a group of 25, every line after is a group of 26
                        // we need to make sure we move to the next line when the group finishes
                        if (keysString == "")
                            keysString = reader.ReadLine().Substring(1); // removes tab

                        pTo = keysString.IndexOf(" ");
                        string keyTimeString = keysString.Substring(0, pTo);
                        float keyTime = Convert.ToSingle(keyTimeString);
                        keysString = keysString.Substring(pTo + 1);

                        pTo = keysString.IndexOf(" ");
                        string keyWeightString = keysString.Substring(0, pTo);
                        float keyWeight = Convert.ToSingle(keyWeightString);
                        keysString = keysString.Substring(pTo + 1);

                        file.facial_animation_curves[(int)curveType].curve_keys[keyIndex].slope_in = 0.0f;
                        file.facial_animation_curves[(int)curveType].curve_keys[keyIndex].slope_out = 0.0f;
                        file.facial_animation_curves[(int)curveType].curve_keys[keyIndex].time = keyTime;
                        file.facial_animation_curves[(int)curveType].curve_keys[keyIndex].weight = keyWeight;

                        if (keyTime < lowestTime)
                            lowestTime = keyTime;
                        if (keyTime > highestTime)
                            highestTime = keyTime;
                    }
                    if (keyCount == 0)
                        reader.ReadLine();
                    reader.ReadLine();
                    reader.ReadLine();
                    reader.ReadLine();
                    reader.ReadLine();
                    reader.ReadLine();
                }
                file.start_time = lowestTime;
                file.end_time = highestTime;
            }
        }
        public void WriteTo(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(file.export_version);
                writer.Write(file.unknown);
                writer.Write(file.checksum);
                writer.Write(file.start_time);
                writer.Write(file.end_time);
                writer.Write(file.blend_in);
                writer.Write(file.blend_out);
                writer.Write(file.unknown1);
                writer.Write(file.unknown2);
                writer.Write(file.unknown3);

                for (int curveIndex = 0; curveIndex < (int)ECurveType.KCurveCount; curveIndex++)
                {
                    writer.Write(file.facial_animation_curves[curveIndex].key_count);
                    writer.Write(file.facial_animation_curves[curveIndex].unknown);
                    for (int keyIndex = 0; keyIndex < file.facial_animation_curves[curveIndex].key_count; keyIndex++)
                    {
                        writer.Write(file.facial_animation_curves[curveIndex].curve_keys[keyIndex].time);
                        writer.Write(file.facial_animation_curves[curveIndex].curve_keys[keyIndex].weight);
                        writer.Write(file.facial_animation_curves[curveIndex].curve_keys[keyIndex].slope_in);
                        writer.Write(file.facial_animation_curves[curveIndex].curve_keys[keyIndex].slope_out);
                    }
                }
            }
        }
    }
}
