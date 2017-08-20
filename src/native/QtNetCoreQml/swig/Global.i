enum NetVariantTypeEnum {
    NetVariantTypeEnum_Invalid,
    NetVariantTypeEnum_Bool,
    NetVariantTypeEnum_Int,
    NetVariantTypeEnum_Double,
    NetVariantTypeEnum_String,
    NetVariantTypeEnum_DateTime,
    NetVariantTypeEnum_Object
};

%typemap(ctype)  NetGCHandle* "void *"
%typemap(imtype) NetGCHandle* "System.IntPtr"
%typemap(cstype) NetGCHandle* "System.IntPtr"
%typemap(csin)   NetGCHandle* "$csinput"
%typemap(in)     NetGCHandle* %{ $1 = $input; %}
%typemap(out)    NetGCHandle* %{ $result = $1; %}
%typemap(csout, excode=SWIGEXCODE)  NetGCHandle* { 
    System.IntPtr cPtr = $imcall;$excode
    return cPtr;
    }
%typemap(csdirectorin) NetGCHandle* "$iminput"

%typemap(ctype)  NetGCHandle** "void **"
%typemap(imtype) NetGCHandle** "ref System.IntPtr"
%typemap(cstype) NetGCHandle** "ref System.IntPtr"
%typemap(csin)   NetGCHandle** "ref $csinput"
%typemap(in)     NetGCHandle** %{ $1 = $input; %}
%typemap(out)    NetGCHandle** %{ $result = $1; %}
%typemap(csout, excode=SWIGEXCODE)  NetGCHandle** { 
    System.IntPtr cPtr = $imcall;$excode
    return cPtr;
    }
%typemap(csdirectorin) NetGCHandle** "ref $iminput"
%typemap(directorin) NetGCHandle** "$input = (void **) $1;"