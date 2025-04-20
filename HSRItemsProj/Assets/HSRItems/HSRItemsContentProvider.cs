using RoR2.ContentManagement;
using UnityEngine;
using RoR2;
using System.Collections;
using RoR2.ExpansionManagement;
namespace HSRItems
{
    public class HSRItemsContent : IContentPackProvider
    {
        public string identifier => HSRItemsMain.GUID;

        public static ReadOnlyContentPack readOnlyContentPack => new ReadOnlyContentPack(HSRItemsContentPack);
        internal static ContentPack HSRItemsContentPack { get; } = new ContentPack();

        public static ItemDef numbyItem;
        public static AssetBundle numbyBundle;
        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            var asyncOperation = AssetBundle.LoadFromFileAsync(HSRItemsMain.assetBundleDir);
            while(!asyncOperation.isDone)
            {
                args.ReportProgress(asyncOperation.progress);
                yield return null;
            }

            //Write code here to initialize your mod post assetbundle load
            
            numbyBundle = asyncOperation.assetBundle;
            numbyItem = numbyBundle.LoadAsset<ItemDef>("Numby");
            var expansionDef = numbyBundle.LoadAsset<ExpansionDef>("HSRItemsExpansion");

            HSRItemsContentPack.itemDefs.Add(new ItemDef[] { numbyItem });
            HSRItemsContentPack.expansionDefs.Add(new ExpansionDef[] { expansionDef });
        }
        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
             ContentPack.Copy(HSRItemsContentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }
        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            //will add more here, first it needs to load. Will add a hit every 'x' hits on 'y' cooldown
            R2API.RecalculateStatsAPI.GetStatCoefficients += (body, statArgs) =>
            {
                if (!body.inventory)
                    return;

                int count = body.inventory.GetItemCount(numbyItem);
                statArgs.critDamageMultAdd += 0.1f * count;
            };
            args.ReportProgress(1f);
            yield break;
        }
        private void AddSelf(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }
        internal HSRItemsContent()
        {
            ContentManager.collectContentPackProviders += AddSelf;
        }
    }
}
