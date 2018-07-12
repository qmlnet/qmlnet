#!/bin/bash
swig3.0 -csharp -c++ -namespace Qt.NetCore -outfile Interop.cs -outdir interop -o native/QtNetCoreQml/swig.cpp -oh native/QtNetCoreQml/swig.h native/QtNetCoreQml/swig/QtNetCoreQml.i
