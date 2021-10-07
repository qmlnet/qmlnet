namespace Qml.Net {
    public class QAbstractListModel : QAbstractItemModel
    {
        public QAbstractListModel() : base()
        {
        }
        public override QModelIndex Parent(QModelIndex _child) {
            return QModelIndex.BlankIndex();
        }
        public override int ColumnCount(QModelIndex parent) {
            return 1;
        }
    }
}