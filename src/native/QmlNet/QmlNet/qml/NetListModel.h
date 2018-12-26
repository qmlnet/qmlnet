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

    static NetListModel* fromReference(const QSharedPointer<NetReference>& reference);

    QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const;
    int rowCount(const QModelIndex &parent = QModelIndex()) const;
    QHash<int,QByteArray> roleNames() const;

    Q_INVOKABLE QVariant at(int index);
    Q_INVOKABLE int length();

private:
    QSharedPointer<NetTypeArrayFacade> _facade;
    QSharedPointer<NetReference> _reference;
};

#endif // NETLISTMODEL_H
