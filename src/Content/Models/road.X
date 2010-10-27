xof 0303txt 0032
template KeyValuePair {
 <26e6b1c3-3d4d-4a1d-a437-b33668ffa1c2>
 STRING key;
 STRING value;
}

template Frame {
 <3d82ab46-62da-11cf-ab39-0020af71e433>
 [...]
}

template Matrix4x4 {
 <f6f23f45-7686-11cf-8f52-0040333594a3>
 array FLOAT matrix[16];
}

template FrameTransformMatrix {
 <f6f23f41-7686-11cf-8f52-0040333594a3>
 Matrix4x4 frameMatrix;
}

template ObjectMatrixComment {
 <95a48e28-7ef4-4419-a16a-ba9dbdf0d2bc>
 Matrix4x4 objectMatrix;
}

template Vector {
 <3d82ab5e-62da-11cf-ab39-0020af71e433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template MeshFace {
 <3d82ab5f-62da-11cf-ab39-0020af71e433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template Mesh {
 <3d82ab44-62da-11cf-ab39-0020af71e433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

template Coords2d {
 <f6f23f44-7686-11cf-8f52-0040333594a3>
 FLOAT u;
 FLOAT v;
}

template MeshTextureCoords {
 <f6f23f40-7686-11cf-8f52-0040333594a3>
 DWORD nTextureCoords;
 array Coords2d textureCoords[nTextureCoords];
}

template ColorRGBA {
 <35ff44e0-6c7c-11cf-8f52-0040333594a3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template ColorRGB {
 <d3e16e81-7835-11cf-8f52-0040333594a3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
}

template Material {
 <3d82ab4d-62da-11cf-ab39-0020af71e433>
 ColorRGBA faceColor;
 FLOAT power;
 ColorRGB specularColor;
 ColorRGB emissiveColor;
 [...]
}

template MeshMaterialList {
 <f6f23f42-7686-11cf-8f52-0040333594a3>
 DWORD nMaterials;
 DWORD nFaceIndexes;
 array DWORD faceIndexes[nFaceIndexes];
 [Material <3d82ab4d-62da-11cf-ab39-0020af71e433>]
}


KeyValuePair {
 "Date";
 "2010-10-22 16:49:42";
}

KeyValuePair {
 "File";
 "";
}

KeyValuePair {
 "User";
 "Houp";
}

KeyValuePair {
 "CoreTime";
 "0";
}

Frame road {
 

 FrameTransformMatrix relative {
  1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.515049,0.000000,0.000000,0.000000,0.000000,1.000000;;
 }

 ObjectMatrixComment object {
  1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
 }

 Mesh mesh_road {
  14;
  -0.512821;0.000000;-0.102564;,
  -0.512821;0.000000;0.102564;,
  0.512821;0.000000;0.102564;,
  0.512821;0.000000;-0.102564;,
  -0.512821;0.000000;-0.102564;,
  0.512821;0.000000;-0.102564;,
  0.512821;0.000000;0.102564;,
  -0.512821;0.000000;0.102564;,
  -0.512821;0.000000;-0.102564;,
  0.512821;0.000000;-0.102564;,
  0.512821;0.000000;0.102564;,
  0.512821;0.000000;0.102564;,
  -0.512821;0.000000;0.102564;,
  -0.512821;0.000000;0.102564;;
  12;
  3;0,1,2;,
  3;2,3,0;,
  3;4,5,6;,
  3;6,7,4;,
  3;8,3,9;,
  3;9,4,8;,
  3;3,10,11;,
  3;11,9,3;,
  3;10,12,13;,
  3;13,11,10;,
  3;12,8,4;,
  3;4,13,12;;

  MeshTextureCoords tc0 {
   14;
   1.000000;1.000000;,
   1.000000;0.000000;,
   0.000000;0.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   1.000000;1.000000;,
   1.000000;0.000000;,
   0.000000;0.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   1.000000;1.000000;,
   1.000000;1.000000;,
   1.000000;1.000000;,
   1.000000;1.000000;;
  }

  MeshMaterialList mtls {
   1;
   12;
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0;

   Material Dflt_Material {
    0.243137;0.101961;0.074510;1.000000;;
    16.000000;
    1.000000;1.000000;1.000000;;
    0.000000;0.000000;0.000000;;
   }
  }
 }
}