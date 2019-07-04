#include <QmlNet/qml/NetQObjectArg.h>
#include <QmlNet/qml/NetVariant.h>
#include <QDebug>

NetQObjectArg::NetQObjectArg() :
    _metaTypeId(QMetaType::Void),
    _data(nullptr)
{
    pack();
}

NetQObjectArg::NetQObjectArg(const int metaTypeId, QSharedPointer<NetVariant> netVariant) :
    _metaTypeId(metaTypeId),
    _data(nullptr),
    _netVariant(netVariant)
{
    pack();
}

NetQObjectArg::~NetQObjectArg()
{
}

QGenericArgument NetQObjectArg::genericArgument()
{
    if(_metaTypeId == QMetaType::Void) {
        return QGenericArgument();
    }
    if(_metaTypeId == QMetaType::QVariant) {
        return QGenericArgument("QVariant", &_variant);
    }
    return QGenericArgument(_variant.typeName(), _variant.data());
}

QGenericReturnArgument NetQObjectArg::genericReturnArguemnet()
{
    if(_metaTypeId == QMetaType::Void) {
        return QGenericReturnArgument();
    }
    if(_metaTypeId == QMetaType::QVariant) {
        return QGenericReturnArgument("QVariant", &_variant);
    }
    return QGenericReturnArgument(_variant.typeName(), _variant.data());
}

QSharedPointer<NetVariant> NetQObjectArg::getNetVariant()
{
    return _netVariant;
}

void NetQObjectArg::pack()
{
    if(_netVariant == nullptr) {
        switch(_metaTypeId) {
        case QMetaType::Void:
            break;
        case QMetaType::QVariant:
            _variant = QVariant();
            break;
        default:
            _variant = QVariant(_metaTypeId, nullptr);
            break;
        }
        return;
    }

    switch(_metaTypeId) {
    case QMetaType::Void:
        break;
    case QMetaType::QVariant:
        _variant = _netVariant->toQVariant();
        break;
    case QMetaType::Bool:
        _variant = QVariant::fromValue(_netVariant->getBool());
        break;
    case QMetaType::QChar:
        _variant = QVariant::fromValue(_netVariant->getChar());
        break;
    case qMetaTypeId<qint32>():
        _variant = QVariant::fromValue(_netVariant->getInt());
        break;
    case qMetaTypeId<quint32>():
        _variant = QVariant::fromValue(_netVariant->getUInt());
        break;
    case qMetaTypeId<qint64>():
        _variant = QVariant::fromValue(_netVariant->getLong());
        break;
    case qMetaTypeId<quint64>():
        _variant = QVariant::fromValue(_netVariant->getULong());
        break;
    case QMetaType::Long:
        _variant = QVariant::fromValue<long>(static_cast<long>(_netVariant->getLong()));
        break;
    case QMetaType::ULong:
       _variant = QVariant::fromValue<ulong>(static_cast<ulong>(_netVariant->getULong()));
        break;
    case QMetaType::Float:
        _variant = QVariant::fromValue(_netVariant->getFloat());
        break;
    case QMetaType::Double:
        _variant = QVariant::fromValue(_netVariant->getDouble());
        break;
    case QMetaType::QString:
        _variant = QVariant::fromValue(_netVariant->getString());
        break;
    case QMetaType::QByteArray:
        _variant = QVariant::fromValue(_netVariant->getBytes());
        break;
    case QMetaType::QDateTime:
        _variant = QVariant::fromValue(_netVariant->getDateTime());
        break;
    case QMetaType::QVariantList:
        _variant = QVariant::fromValue<QVariantList>(_netVariant->toQVariantList());
        break;
    case QMetaType::QObjectStar:
        switch(_netVariant->getVariantType()) {
        case NetVariantTypeEnum_Invalid:
            _variant = QVariant::fromValue<QObject*>(nullptr);
            break;
        case NetVariantTypeEnum_Object:
        case NetVariantTypeEnum_QObject:
            _variant = _netVariant->toQVariant();
            break;
        default:
            qWarning() << "Unabled to convert " << _netVariant->getVariantType() << " to QObject*";
            break;
        }
        break;
    default:
        QMetaType::TypeFlags flags = QMetaType::typeFlags(_metaTypeId);
        if(flags & QMetaType::PointerToQObject) {
            // If the netvariant is a QObject and is of the same type,
            // let's use it.
            QVariant possibleQObjectVariant = _netVariant->toQVariant();
            if(possibleQObjectVariant.userType() == QMetaType::QObjectStar) {
                QObject* value = possibleQObjectVariant.value<QObject*>();
                if(value == nullptr) {
                    _variant = QVariant(_metaTypeId, nullptr);
                    break;
                }

                const QMetaObject* targetMetaObject = QMetaType::metaObjectForType(_metaTypeId);

                QObject* casted = targetMetaObject->cast(value);

                if(casted == nullptr) {
                    qWarning() << "Can't convert " << value->metaObject()->className() << "to" << QMetaType::typeName(_metaTypeId);
                    _variant = QVariant(_metaTypeId, nullptr);
                    break;
                }

                _variant = qVariantFromValue(casted);

                break;
            }
        }

        qWarning() << "Unsupported type: " << QMetaType::typeName(_metaTypeId);
        _variant = QVariant(_metaTypeId, nullptr);
        break;
    }
}

void NetQObjectArg::unpack()
{
    if(_metaTypeId == QMetaType::Void) {
        return;
    }
    if(_netVariant == nullptr) {
        _netVariant = QSharedPointer<NetVariant>(new NetVariant());
    }
    NetVariant::fromQVariant(&_variant, _netVariant);
}
