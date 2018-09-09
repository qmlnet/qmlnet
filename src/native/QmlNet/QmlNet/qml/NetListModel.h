#ifndef NETLISTMODEL_H
#define NETLISTMODEL_H

#include <QAbstractListModel>
#include <QSharedPointer>

class NetTypeArrayFacade;
class NetReference;

class NetListModel : public QAbstractListModel
{
    Q_OBJECT
public:
    NetListModel(QObject* parent, QSharedPointer<NetTypeArrayFacade> facade, QSharedPointer<NetReference> reference);

    static NetListModel* fromReference(QSharedPointer<NetReference> reference);

    QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const;
    int rowCount(const QModelIndex &parent = QModelIndex()) const;
    QHash<int,QByteArray> roleNames() const;

private:
    QSharedPointer<NetTypeArrayFacade> _facade;
    QSharedPointer<NetReference> _reference;
};

#endif // NETLISTMODEL_H
