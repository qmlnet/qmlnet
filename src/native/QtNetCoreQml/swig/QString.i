// Reference
%typemap(ctype) QString& "char*"
%typemap(imtype,
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]")
    QString& "string"
%typemap(cstype) QString& "string"
%typemap(in) QString& (QString temp) %{
    if($input) {
        temp = $input;
    }
    $1 = &temp;
%}
%typemap(out) QString& %{
    if(!$1->isNull()) {
        $result = (char*)SWIG_csharp_string_callback($1->toUtf8().data());
    } else {
        $result = NULL;
    }
%}
%typemap(csin) QString& "$csinput"
%typemap(csout) QString& {
    string ret = $imcall;
    return ret;
  }

// Value
%typemap(ctype) QString "char*"
%typemap(imtype,
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]")
    QString "string"
%typemap(cstype) QString "string"
%typemap(in) QString (QString temp) %{
    if($input) {
        $1 = $input;
    }
%}
%typemap(out) QString %{
    if(!$1.isNull()) {
        $result = (char*)SWIG_csharp_string_callback($1.toUtf8().data());
    } else {
        $result = NULL;
    }
%}
%typemap(csin) QString "$csinput"
%typemap(csout) QString {
    string ret = $imcall;
    return ret;
  }

// Pointer
%typemap(ctype) QString* "char*"
%typemap(imtype,
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]")
    QString* "string"
%typemap(cstype) QString* "string"
%typemap(in) QString* (QString temp) %{
    if($input) {
        temp = $input;
    }
    $1 = &temp;
%}
%typemap(out) QString* %{
    if(!$1->isNull()) {
        $result = (char*)SWIG_csharp_string_callback($1->toUtf8().data());
    } else {
        $result = NULL;
    }
%}
%typemap(csin) QString* "$csinput"
%typemap(csout) QString* {
    string ret = $imcall;
    return ret;
  }