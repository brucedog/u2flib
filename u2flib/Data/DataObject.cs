﻿/*
 * Code was copied from java example so there is a .NET version 
 * to work with.
 * 
 * Copyright 2014 Yubico.
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using System;
using Newtonsoft.Json;

namespace u2flib.Data
{
    public abstract class DataObject
    {
        public String ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T FromJson<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}