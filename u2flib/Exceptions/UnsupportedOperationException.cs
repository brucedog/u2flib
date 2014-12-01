/*
 * Copyright 2014 Yubico.
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using System;
using Org.BouncyCastle.Security;

namespace u2flib.Exceptions
{
    public class UnsupportedOperationException : Exception
    {
        public UnsupportedOperationException(string errorWhenComputingSha, Exception noSuchAlgorithmException)
        {
            Console.WriteLine("Error computing sha:{0} No such algorithem exception:{1}", errorWhenComputingSha, noSuchAlgorithmException);
        }
    }
}