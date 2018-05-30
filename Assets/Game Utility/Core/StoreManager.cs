using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreManager : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    public List<IAPProduct> products;
    private void OnEnable()
    {
        GameUtility.Instance.OnPurchaseProduct += BuyProductID;
    }

    private void OnDisable()
    {
        GameUtility.Instance.OnPurchaseProduct -= BuyProductID;
    }

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //foreach (var item in products)
        //{
        //    builder.AddProduct(item.id, item.type);
        //}
        //UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                DebugText.instance.Print(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                DebugText.instance.Print("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            DebugText.instance.Print("BuyProductID FAIL. Not initialized.");
        }
    }


    public void RestorePurchases()
    {
        //if (!IsInitialized())
        //{
        //    DebugText.instance.Print("RestorePurchases FAIL. Not initialized.");
        //    return;
        //}

        //if (Application.platform == RuntimePlatform.IPhonePlayer ||
        //    Application.platform == RuntimePlatform.OSXPlayer)
        //{
        //    DebugText.instance.Print("RestorePurchases started ...");

        //    var apple = m_StoreExtensionProvider.GetExtension();
        //    apple.RestoreTransactions((result) =>
        //    {
        //        DebugText.instance.Print("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
        //    });
        //}
        //else
        //{
        //    DebugText.instance.Print("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        //}
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        DebugText.instance.Print("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        DebugText.instance.Print("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        GameUtility.Instance.ProductPurchased(args);
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        DebugText.instance.Print(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
