// BLL "Business Logic Layer"
// All the classes in the BLL layer should handle the application's
// use of the databaselayer "DAL". The BLL layer should only be accessed 
// from the BlissBase "web application" and not the DAL or Models layer.
// The BLL layer should be the only connection down to the DAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlissBase.DAL;
using BlissBase.Model;

namespace BlissBase.BLL
{
    public class CompositeScoreBLL
    {
        bool testing;
        public CompositeScoreBLL()
        {
            testing = false;
        }
        public CompositeScoreBLL(bool testing_)
        {
            testing = testing_;
        }
        public int GetScoreFor(int id)
        {
            var CompositeScoreDAL = new CompositeScoreDAL(testing);
            return CompositeScoreDAL.GetScoreFor(id);
        }

        public int UpdateScoreFor(int id, int difference)
        {
            var CompositeScoreDAL = new CompositeScoreDAL(testing);
            return CompositeScoreDAL.UpdateScoreFor(id, difference);
        }

        public bool RemoveScoreFor(int id)
        {
            var CompositeScoreDAL = new CompositeScoreDAL(testing);
            return CompositeScoreDAL.RemoveScoreFor(id);
        }
    }
}
