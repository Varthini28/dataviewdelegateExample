using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LS.Integration.Core;
using LS.Starlims.Acu.Attributes;
using LS.Starlims.Acu.DAC;
using LS.Starlims.Acu.DAC.Filter;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.PO;

namespace LS.Starlims.Acu
{
    public class ProcessSTARLIMSDataProc : PXGraph<ProcessSTARLIMSDataProc>
    {
        public PXCancel<LSPBStarlimsProcFilter> Cancel;

        public PXFilter<LSPBStarlimsProcFilter> Filter;

        public SelectFrom<LSPBStarlimsEntity>
            .Where<LSPBStarlimsEntity.processed.IsEqual<False>
                .And<
                    Brackets<LSPBStarlimsEntity.integrationEntityType.IsEqual<
                            LSPBStarlimsProcFilter.entityType.FromCurrent>
                        .Or<LSPBStarlimsProcFilter.entityType.FromCurrent.IsNull>>>>
            .OrderBy<Asc<LSPBStarlimsEntity.integrationEntityType>>
            .ProcessingView.FilteredBy<LSPBStarlimsProcFilter> Entities;

        protected virtual IEnumerable entities() //dataviewdelgate
        {
            var filter = Entities.Current;

            var query = new PXSelect<LSPBStarlimsEntity>(this);
           
            var list = query.Select();

            foreach (LSPBStarlimsEntity record in list)
            {
              
                if (record.IntegrationEntityType == StarLimsEntityType.NonStockItem)
                {
                    InventoryItem res = PXSelect<InventoryItem, Where<InventoryItem.noteID, Equal<Required<InventoryItem.noteID>>>>.Select(this, record.RefNoteId);
                   if(res != null)
                    {
                        record.StarlimsKey = res.Descr;
                        yield return record;
                    }
                   
                }
                else
                {                    
                    yield return record;
                }
                yield return record;
            }
        }

}
}
