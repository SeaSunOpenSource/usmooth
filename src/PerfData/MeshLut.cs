/*!lic_info

The MIT License (MIT)

Copyright (c) 2015 SeaSunOpenSource

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using usmooth.common;

public class MeshLut {
	public bool AddMesh(GameObject go) {
		if (_lut.ContainsKey(go.GetInstanceID())) {
			return true;
		}

		// returns false if renderer is not available
		if (go.renderer == null) {
			return false;
		}
		
		// returns false if not a mesh
		MeshFilter mf = (MeshFilter)go.GetComponent (typeof(MeshFilter));
		if (mf == null) {
			return false;
		}

		MeshData md = new MeshData ();
		md._instID = go.GetInstanceID ();
		md._vertCount = mf.mesh.vertexCount;
		md._triCount = mf.mesh.triangles.Length / 3;
		md._materialCount = go.renderer.sharedMaterials.Length;
		md._boundSize = go.renderer.bounds.size.magnitude;
		_lut.Add (md._instID, md);
		return true;
	}

	public void WriteMesh(int instID, UsCmd cmd) {
		MeshData data;
		if (_lut.TryGetValue(instID, out data)) {
			data.Write(cmd);
		}
	}

	Dictionary<int, MeshData> _lut = new Dictionary<int, MeshData>();
}
