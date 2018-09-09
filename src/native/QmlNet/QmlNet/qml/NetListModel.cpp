#include <QmlNet/qml/NetListModel.h>
#include <QmlNet/types/NetReference.h>
#include <QmlNet/types/NetTypeArrayFacade.h>
#include <QmlNet/qml/NetVariant.h>

NetListModel::NetListModel(
    QObject* parent,
    QSharedPointer<NetTypeArrayFacade> facade,
    QSharedPointer<NetReference> reference) :
    QAbstractListModel(parent),
    _facade(facade),
    _reference(reference)
{

}

NetListModel* NetListModel::fromReference(QSharedPointer<NetReference> reference)
{
    QSharedPointer<NetTypeArrayFacade> facade = reference->getTypeInfo()->getArrayFacade();
    if(facade == nullptr) {
        return nullptr;
    }
    return new NetListModel(nullptr, facade, reference);
}

QVariant NetListModel::data(const QModelIndex &index, int role) const
{
    if(role != 0) {
        qWarning("invalid role id: %d", role);
        return QVariant();
    }
    int length = static_cast<int>(_facade->getLength(_reference));
    if (index.row() < 0 || index.row() >= length)
            return QVariant();

    QSharedPointer<NetVariant> result = _facade->getIndexed(_reference, static_cast<uint>(index.row()));
    if(result == nullptr) {
        return QVariant();
    }
    return result->toQVariant();
}

int NetListModel::rowCount(const QModelIndex &parent) const
{
    Q_UNUSED(parent);
    return static_cast<int>(_facade->getLength(_reference));
}

QHash<int,QByteArray> NetListModel::roleNames() const
{
    QHash<int, QByteArray> roles;
    roles[0] = "modelData";
    return roles;
}
