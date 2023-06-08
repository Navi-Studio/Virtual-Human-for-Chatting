/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at http: //live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using System;
using System.Collections.Generic;
using System.Text;


namespace Live2D.Cubism.Framework.Json
{
    /// <summary>
    /// Cubism json parser for loading the configuration file etc.
    ///
    /// Minimal lightweight JSON parser that only supports Ascii characters.
    /// Specification is a subset of JSON.
    ///
    /// Unsupported item.
    /// - Non-ASCII characters such as Japanese.
    /// - Exponential representation by e.
    /// </summary>
    public class CubismJsonParser
    {
        #region variable

        /// <summary>
        /// Array of buffer.
        /// </summary>
        private char[] buffer;

        /// <summary>
        /// Length of buffer.
        /// </summary>
        private int length;

        /// <summary>
        /// For error message.
        /// </summary>
        private int line_count = 0;

        /// <summary>
        /// Root node.
        /// </summary>
        private Value root;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name"jsonBytes">Byte data.</param>
        public CubismJsonParser(char[] jsonBytes)
        {
            this.buffer = jsonBytes;
            this.length = jsonBytes.Length;
        }

        #region Parse Functionn

        /// <summary>
        /// Parse JSON.
        /// </summary>
        /// <returns>Value of parsed from JSON.</returns>
        public Value Parse()
        // throws Exception.
        {
            try
            {
                var ret = new int[1];
                root = ParseValue(buffer, length, 0, ret);
                return root;
            }
            catch (Exception e)
            {
                throw new Exception("json error " + "@line:" + line_count + " / " + e.Message, e);
            }
        }


        /// <summary>
        /// Parse JSON from byte data.
        /// </summary>
        /// <param name="jsonBytes">Byte data.</param>
        /// <returns>Value of parsed from JSON.</returns>
        public static Value ParseFromBytes(char[] jsonBytes)
        // throws Exception.
        {
            var jp = new CubismJsonParser(jsonBytes);
            var ret = jp.Parse();
            return ret;
        }


        /// <summary>
        /// Parse JSON from string data.
        /// </summary>
        /// <param name="jsonString">String data.</param>
        /// <returns>Value of parsed from JSON.</returns>
        public static Value ParseFromString(String jsonString)
        // throws Exception.
        {
            var buffer = jsonString.ToCharArray();
            var jp = new CubismJsonParser(buffer);
            var ret = jp.Parse();
            return ret;
        }


        /// <summary>
        /// Parse till next.
        /// </summary>
        /// <param name="buffer">json data buffer.</param>
        /// <param name="length">json data buffer length.</param>
        /// <param name="pos">Parse position.</param>
        /// <param name="endPos">End position.</param>
        /// <returns>String of parsed from JSON.</returns>
        private static String ParseString(char[] str, int length, int pos, int[] endPos)
        // throws Exception.
        {
            char c, c2;
            StringBuilder stringBuffer = null;
            var startPos = pos; // start pos of the word which is not in sbuf

            for (var i = pos; i < length; i++)
            {
                c = (char)(str[i] & 0xFF);

                switch (c)
                {
                    case '\"': // end " , escape char never comes here.
                        endPos[0] = i + 1; // next word of "
                        if (stringBuffer != null)
                        {
                            if (i - 1 > startPos) stringBuffer.Append(new string(str, startPos, i - 1 - startPos)); // regist till prev char
                            return stringBuffer.ToString();
                        }
                        else
                        {
                            return new string(str, pos, i - pos);
                        }

                    case '\\': // escape
                        if (stringBuffer == null)
                        {
                            stringBuffer = new StringBuilder();
                        }
                        if (i > startPos) stringBuffer.Append(new string(str, startPos, i - startPos)); // regist till prev char

                        i++; // 2 chars

                        if (i < length)
                        {
                            c2 = (char)(str[i] & 0xFF);
                            switch (c2)
                            {
                                case '\\': stringBuffer.Append('\\'); break;
                                case '\"': stringBuffer.Append('\"'); break;
                                case '/': stringBuffer.Append('/'); break;
                                case 'b': stringBuffer.Append('\b'); break;
                                case 'f': stringBuffer.Append('\f'); break;
                                case 'n': stringBuffer.Append('\n'); break;
                                case 'r': stringBuffer.Append('\r'); break;
                                case 't': stringBuffer.Append('\t'); break;
                                case 'u':
                                    throw new Exception("parse string/unicode escape not supported");
                            }
                        }
                        else
                        {
                            throw new Exception("parse string/escape error");
                        }
                        startPos = i + 1; // after next to escape char (2chars)
                        break;
                }
            }
            throw new Exception("parse string/illegal end");
        }


        /// <summary>
        /// Parse object, not include { at pos.
        /// </summary>
        /// <param name="buffer">json data buffer.</param>
        /// <param name="length">json data buffer length.</param>
        /// <param name="pos">Parse position.</param>
        /// <param name="endPos">End position.</param>
        /// <returns>Value of parsed from JSON.</returns>
        private Value ParseObject(char[] buffer, int length, int pos, int[] endPos)
        // throws Exception.
        {
            var ret = new Dictionary<String, Value>();

            // key : value ,
            String key = null;
            char c;
            var i = pos;
            var ret_endPos = new int[1];
            var ok = false;

            // loop till , is lasting
            for (; i < length; i++)
            {
                // FOR_LOOP1:
                for (; i < length; i++)
                {
                    c = (char)(buffer[i] & 0xFF);

                    switch (c)
                    {
                        case '\"':
                            key = ParseString(buffer, length, i + 1, ret_endPos);
                            i = ret_endPos[0];
                            ok = true;
                            goto EXIT_FOR_LOOP1;
                        case '}': endPos[0] = i + 1; return new Value(ret); // empty
                        case ':': throw new Exception("illegal ':' position");
                        default: break; // skip char
                    }
                }
                EXIT_FOR_LOOP1:

                if (!ok)
                {
                    throw new Exception("key not found");
                }
                ok = false;

                // check :
                // FOR_LOOP2:
                for (; i < length; i++)
                {
                    c = (char)(buffer[i] & 0xFF);

                    switch (c)
                    {
                        case ':': ok = true; i++; goto EXIT_FOR_LOOP2;
                        case '}': throw new Exception("illegal '}' position");
                        case '\n': line_count++; break;
                        default: break; // skip char
                    }
                }
                EXIT_FOR_LOOP2:

                if (!ok)
                {
                    throw new Exception("':' not found");
                }

                // check :
                Value value = ParseValue(buffer, length, i, ret_endPos);
                i = ret_endPos[0];
                ret.Add(key, value);

                // FOR_LOOP3:
                for (; i < length; i++)
                {
                    c = (char)(buffer[i] & 0xFF);

                    switch (c)
                    {
                        case ',': goto EXIT_FOR_LOOP3; // next key, value
                        case '}': endPos[0] = i + 1; return new Value(ret); //finished
                        case '\n': line_count++; break;
                        default: break; // skip
                    }
                }
                EXIT_FOR_LOOP3: ;

            }

            throw new Exception("illegal end of ParseObject");
        }


        /// <summary>
        /// Parse Array, not include first[ at pos.
        /// </summary>
        /// <param name="buffer">json data buffer.</param>
        /// <param name="length">json data buffer length.</param>
        /// <param name="pos">Parse position.</param>
        /// <param name="endPos">End position.</param>
        /// <returns>Value of parsed from JSON.</returns>
        private Value ParseArray(char[] buffer, int length, int pos, int[] endPos)
        // throws Exception.
        {
            var ret = new List<Value>();
            var i = pos;
            char c;
            var ret_endPos = new int[1];

            // loop till, is lasting
            for (; i < length; i++)
            {
                // check :
                var value = ParseValue(buffer, length, i, ret_endPos);
                i = ret_endPos[0];
                if (value != null)
                {
                    ret.Add(value);
                }

                // FOR_LOOP3:
                for (; i < length; i++)
                {
                    c = (char)(buffer[i] & 0xFF);

                    switch (c)
                    {
                        case ',': goto EXIT_FOR_LOOP3; // next key value
                        case ']': endPos[0] = i + 1; return new Value(ret); // finish
                        case '\n': line_count++; break;
                        default: break; // skip
                    }
                }
                EXIT_FOR_LOOP3: ;
            }

            throw new Exception("illegal end of ParseObject");
        }


        /// <summary>
        /// Parse double.
        /// </summary>
        /// <param name="buffer">json data buffer.</param>
        /// <param name="length">json data buffer length.</param>
        /// <param name="pos">Parse position.</param>
        /// <param name="endPos">End position.</param>
        /// <returns>Double of parsed from JSON.</returns>
        public static double strToDouble(char[] str, int length, int pos, int[] endPos)
        {
            // int length = str.length ;
            var i = pos;
            var minus = false; // minus flag
            var period = false;
            var v1 = 0.0;

            // check minus
            var c = (char)(str[i] & 0xFF);
            if(c == '-')
            {
                minus = true;
                i++;
            }

            // check integer part
            // FOR_LOOP:
            for (; i < length; i++)
            {
                c = (char)(str[i] & 0xFF);

                switch (c)
                {
                    case '0': v1 = v1 * 10; break;
                    case '1': v1 = v1 * 10 + 1; break;
                    case '2': v1 = v1 * 10 + 2; break;
                    case '3': v1 = v1 * 10 + 3; break;
                    case '4': v1 = v1 * 10 + 4; break;
                    case '5': v1 = v1 * 10 + 5; break;
                    case '6': v1 = v1 * 10 + 6; break;
                    case '7': v1 = v1 * 10 + 7; break;
                    case '8': v1 = v1 * 10 + 8; break;
                    case '9': v1 = v1 * 10 + 9; break;
                    case '.':
                        period = true;
                        i++;
                        goto EXIT_FOR_LOOP;
                    default: // new line code , and delim
                        goto EXIT_FOR_LOOP;
                }
            }

            EXIT_FOR_LOOP:

            // check floating point part
            if (period)
            {
                var mul = 0.1;

                // FOR_LOOP2:
                for (; i < length; i++)
                {
                    c = (char)(str[i] & 0xFF);

                    switch (c)
                    {
                        case '0': break;
                        case '1': v1 += mul * 1; break;
                        case '2': v1 += mul * 2; break;
                        case '3': v1 += mul * 3; break;
                        case '4': v1 += mul * 4; break;
                        case '5': v1 += mul * 5; break;
                        case '6': v1 += mul * 6; break;
                        case '7': v1 += mul * 7; break;
                        case '8': v1 += mul * 8; break;
                        case '9': v1 += mul * 9; break;
                        default: // new line code, and delim
                            goto EXIT_FOR_LOOP2;
                    }
                    mul *= 0.1;
                }

                EXIT_FOR_LOOP2:;
            }

            if (minus)
            {
                v1 = -v1;
            }

            endPos[0] = i;
            return v1;
        }


        /// <summary>
        /// Parse one Value(float, String, Object, Array, null, true, false).
        /// </summary>
        /// <param name="buffer">json data buffer.</param>
        /// <param name="length">json data buffer length.</param>
        /// <param name="pos">Parse position.</param>
        /// <param name="endPos">End position.</param>
        /// <returns>Value of parsed from JSON.</returns>
        private Value ParseValue(char[] buffer, int length, int pos, int[] endPos)
        // throws Exception.
        {
            Value obj;
            var i = pos;
            for (; i < length; i++)
            {
                var c = (char)(buffer[i] & 0xFF);

                switch (c)
                {
                    case '-':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        var f = strToDouble(buffer, length, i, endPos);
                        return new Value(f);
                    case '\"':
                        obj = new Value(ParseString(buffer, length, i + 1, endPos)); // next to \"
                        return obj;
                    case '[':
                        obj = ParseArray(buffer, length, i + 1, endPos);
                        return obj;
                    case ']': // It is illegal } but skip it. There seems to be unnecessary at the end of the array
                        // obj = null;
                        endPos[0] = i; // Reprocess the same letters
                        return null;
                    case '{':
                        obj = ParseObject(buffer, length, i + 1, endPos);
                        return obj;
                    case 'n': // null
                        if (i + 3 < length) obj = null;
                        else throw new Exception("parse null");
                        return obj;
                    case 't': // true
                        if (i + 3 < length) obj = new Value(true);
                        else throw new Exception("parse true");
                        return obj;
                    case 'f': // false
                        if (i + 4 < length) obj = new Value(false);
                        else throw new Exception("parse false");
                        return obj;
                    case ',': // Array separator
                        throw new Exception("illegal ',' position");
                    case '\n': line_count++;
                        break;
                    case ' ':
                    case '\t':
                    case '\r':
                    default: // skip
                        break;
                }
            }

            // throw new Exception("illegal end of value");
            return null;
        }

        #endregion
    }


    /// <summary>
    /// Json value.
    /// </summary>
    public class Value
    {
        private Object _object;

        /// <summary>
        /// Get value.
        /// </summary>
        /// <returns>The JSON value.</returns>
        public Value(Object obj)
        {
            this._object = obj;
        }

        #region toString

        /// <summary>
        /// Value to string.
        /// </summary>
        /// <returns>Value of string type.</returns>
        public string toString()
        {
            return toString("");
        }

        /// <summary>
        /// Value to string.
        /// </summary>
        /// <returns>Value of string type.</returns>
        public string toString(string indent)
        {
            if (_object is string)
            {
                return (string)_object;
            }

            //------------ List ------------
            else if (_object is List<Value>)
            {
                string ret = indent + "[\n";
                foreach (Value v in ((List<Value>)_object))
                {
                    ret += indent + "    " + v.toString(indent + "    ") + "\n";
                }
                ret += indent + "]\n";
                return ret;
            }

            //------------ Dictionary ------------
            else if (_object is Dictionary<string, Value>)
            {

                string ret = indent + "{\n";
                Dictionary<string, Value> vmap = (Dictionary<string, Value>)_object;
                foreach (KeyValuePair<string, Value> pair in vmap)
                {
                    Value v = pair.Value;
                    ret += indent + "    " + pair.Key + " : " + v.toString(indent + "    ") + "\n";
                }
                ret += indent + "}\n";
                return ret;
            }
            else
            {
                return "" + _object;
            }
        }

        #endregion

        #region toInt

        /// <summary>
        /// Value to int.
        /// </summary>
        /// <returns>Value of int type.</returns>
        public int toInt()
        {
            return toInt(0);
        }

        /// <summary>
        /// Value to int.
        /// </summary>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Value of int type.</returns>
        public int toInt(int defaultValue)
        {
            return (_object is Double) ? (int)((Double)_object) : defaultValue;
        }

        #endregion

        #region ToFloat

        /// <summary>
        /// Value to float.
        /// </summary>
        /// <returns>Value of float type.</returns>
        public float ToFloat()
        {
            return ToFloat(0);
        }

        /// <summary>
        /// Value to float.
        /// </summary>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Value of float type.</returns>
        public float ToFloat(float defaultValue)
        {
            return (_object is Double) ? (float)((Double)_object) : defaultValue;
        }

        #endregion

        #region ToDouble

        /// <summary>
        /// Value to double.
        /// </summary>
        /// <returns>Value of double type.</returns>
        public double ToDouble()
        {
            return ToDouble(0);
        }

        /// <summary>
        /// Value to double.
        /// </summary>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Value of double type.</returns>
        public double ToDouble(double defaultValue)
        {
            return (_object is Double) ? ((Double)_object) : defaultValue;
        }

        #endregion

        #region toArray

        /// <summary>
        /// Get list.
        /// </summary>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Value list.</returns>
        public List<Value> GetVector(List<Value> defalutV)
        {
            return (_object is List<Value>) ? (List<Value>)_object : defalutV;
        }


        /// <summary>
        /// Get from list.
        /// </summary>
        /// <param name="index">Value index in list.</param>
        /// <returns>Value from list.</returns>
        public Value Get(int index)
        {
            return (_object is List<Value>) ? (Value)((List<Value>)_object)[index] : null;
        }

        #endregion

        #region toDictionary

        /// <summary>
        /// Get Value of dictionary type.
        /// </summary>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Value of dictionary type.</returns>
        public Dictionary<string, Value> GetMap(Dictionary<string, Value> defalutV)
        {
            return (_object is Dictionary<string, Value>) ? (Dictionary<string, Value>)_object : defalutV;
        }


        /// <summary>
        /// Get data from dictionary.
        /// </summary>
        /// <param name="key">key.</param>
        /// <returns>Key value from dictionary.</returns>
        public Value Get(string key)
        {
            if(_object is Dictionary<string, Value>)
            {
                if (((Dictionary<string, Value>)_object).ContainsKey(key)) return (Value)((Dictionary<string, Value>)_object)[key];
            }

            return null;
        }


        /// <summary>
        /// Get key list from dictionary.
        /// </summary>
        /// <returns>Key list.</returns>
        public List<string> KeySet()
        {
            return (_object is Dictionary<string, Value>) ? new List<string>(((Dictionary<string, Value>)_object).Keys) : null;
        }


        /// <summary>
        /// Get dictionary.
        /// </summary>
        /// <returns>Value of dictionary type.</returns>
        public Dictionary<string, Value> ToMap()
        {
            return (_object is Dictionary<string, Value>) ? (Dictionary<string, Value>)_object: null;
        }

        #endregion

        #region check type

        /// <summary>
        /// Confirm the type.
        /// </summary>
        public bool isNull()    { return _object == null; }
        public bool isBoolean() { return _object is Boolean; }
        public bool isDouble()  { return _object is Double; }
        public bool isString()  { return _object is string; }
        public bool isArray()   { return _object is List<Value>; }
        public bool isMap()     { return _object is Dictionary<string, Value>; }

        #endregion
    }
}
