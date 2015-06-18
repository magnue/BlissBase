/*
 * References used: https://msdn.microsoft.com/en-us/library/ms182532.aspx
 * If tests are not discovered see this https://connect.microsoft.com/VisualStudio/feedback/details/807771/visual-studio-2013-test-explorer-only-works-if-run-as-administrator
 * SHORT VERSION: RUN VISUAL STUDIO AS ADMINISTRATOR
 */
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlissBase.DAL;

using System.Collections.Generic;
using System.Collections;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Data.SqlClient;

namespace BlissBase.UnitTesting
{
    [TestClass]
    public class UnitTestsDAL
    {
        const bool testing = true;

        [TestMethod]
        public void TestOpenTestDatabase()
        {
            try
            {
                using (var db = new BlissBaseContext(testing))
                {
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.StackTrace);
            }
        }

        [TestMethod]
        public void TestRawImport()
        {
            try
            {
                RawSymbolImportDAL rawImport = new RawSymbolImportDAL(testing);
                rawImport.ImportFromPath("../../../TestData");
            }
            catch (Exception e)
            {
                Assert.Fail(e.StackTrace);
            }
        }
    }
}
