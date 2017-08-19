%typemap(ctype) QString& "wchar_t *"
%typemap(imtype,
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]")
    QString& "string"
%typemap(cstype) QString& "string"
%typemap(in) QString& (QString temp) %{
    temp = QString::fromWCharArray($input);
    $1 = &temp;
%}
%typemap(csin) QString& "$csinput"
%typemap(csout) QString& {
    string ret = $imcall;
    return ret;
  }
  
%typemap(imtype) QString "string"
%typemap(cstype) QString "string"
%typemap(csout) QString {
    string ret = $imcall;
    return ret;
  }
%typemap(out) QString (QString temp) %{
    wchar_t* $1_array = new wchar_t[$1.length() + 1];
    $1.toWCharArray($1_array);
    $1_array[$1.length()] = 0;
    $result = SWIG_csharp_wstring_callback($1_array);
    delete[] $1_array;
%}

%typemap(ctype) QString* "wchar_t *"
%typemap(imtype,
    inattributes="[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]")
    QString* "string"
%typemap(cstype) QString* "string"
%typemap(in) QString* (QString temp) %{
    if($input) {
        temp = QString::fromWCharArray($input);
        $1 = &temp;
    }
%}
%typemap(csin) QString* "$csinput"
%typemap(csout) QString* {
    string ret = $imcall;
    return ret;
  }