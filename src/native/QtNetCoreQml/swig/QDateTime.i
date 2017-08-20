%typemap(ctype) QDateTime& "char *"
%typemap(imtype) QDateTime& "string"
%typemap(cstype) QDateTime& "System.DateTime?"
%typemap(csin) QDateTime& "$csinput.HasValue ? $csinput.Value.ToString(\"o\") : null"
%typemap(in) QDateTime& (QDateTime temp) %{
    if($input) {
        QString $1_s = QString::fromLocal8Bit($input);
        temp = QDateTime::fromString($1_s, Qt::ISODate);
    }
    $1 = &temp;
%}

%typemap(ctype) QDateTime "char *"
%typemap(imtype) QDateTime "string"
%typemap(cstype) QDateTime "System.DateTime?"
%typemap(out) QDateTime %{
    if($1.isNull()) {
        $result = NULL;
    } else if(!$1.isValid()) {
        qDebug() << "Invalid date time";
        $result = NULL;
    } else {
        QString $1_string = $1.toString(Qt::ISODate);
        QByteArray $1_ba = $1_string.toLatin1();
        $result = SWIG_csharp_string_callback($1_ba.data());
    }
%}
%typemap(csout) QDateTime {
    string ret = $imcall;
    if(ret == null) return null;
    return System.DateTime.Parse(ret, null, System.Globalization.DateTimeStyles.RoundtripKind);
}