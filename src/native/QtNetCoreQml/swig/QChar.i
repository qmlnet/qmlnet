%typemap(ctype) QChar "int"
%typemap(imtype) QChar "int"
%typemap(cstype) QChar "char"
%typemap(csin) QChar "(int)$csinput"
%typemap(csout) QChar {
    return (char)$imcall;
}
%typemap(in) QChar %{
    $1 = $input;
%}
%typemap(out) QChar %{
    $result = $1.unicode();
%}
  