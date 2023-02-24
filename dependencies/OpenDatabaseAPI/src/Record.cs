using System.Security.Cryptography.X509Certificates;

namespace OpenDatabase
{
    /// <summary>
    /// Stores a database record's keys and values. 
    /// </summary>
    public struct Record
    {
        public string[] Keys;

        public object[] Values;

        public override string ToString()
        {
            return QueryBuilder.GetRecordTuple(this);
        }

        public Record(string[] keys, object[] values)
        {
            this.Keys = keys;
            this.Values = values;
        }
    }
}
