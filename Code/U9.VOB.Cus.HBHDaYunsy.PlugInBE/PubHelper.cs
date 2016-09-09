using System;
using System.Web;
using System.Xml;
using UFIDA.U9.Base.Profile;
using UFIDA.U9.Base.Profile.Proxy;
using System.Collections.Generic;
using UFIDA.U9.Base;
using UFIDA.U9.CBO.SCM.Warehouse;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.SPR.SalePriceList;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.SM.Ship;
using UFIDA.U9.InvTrans.Trans;
using UFSoft.UBF.PL;
using UFSoft.UBF.Business;

namespace U9.VOB.Cus.HBHDaYunsy.PlugInBE
{
	public class PubHelper
    {
        // ����Դ,���,�۱����
        /// <summary>
        /// ����Դ,���,�۱����
        /// </summary>
        public const string Const_ElectricPartPriceListCode = "SPL2016050007";


        private static List<string> lstPriceList2DMS = new List<string>();
        public static List<string> PriceList2DMS
        {
            get
            {
                if (lstPriceList2DMS.Count == 0)
                {
                    lstPriceList2DMS.Add(Const_ElectricPartPriceListCode);
                }

                return lstPriceList2DMS;
            }
        }

        // ����Դ��֯
        /// <summary>
        /// ����Դ��֯
        /// </summary>
        public const string Const_OrgCode_Electric = "70";
        // ���������������޹�˾
        /// <summary>
        /// ���������������޹�˾
        /// </summary>
        public const string Const_OrgCode_Hubei = "20";
        // �ɶ�����ʮ�߷ֹ�˾
        /// <summary>
        /// �ɶ�����ʮ�߷ֹ�˾
        /// </summary>
        public const string Const_OrgCode_Chengdu = "10";

        // ������֯תDMS
        private static List<string> lstSaleOrg2DMS = new List<string>();
        /// <summary>
        /// ������֯תDMS
        /// </summary>
        public static List<string> SaleOrg2DMS
        {
            get
            {
                if (lstSaleOrg2DMS.Count == 0)
                {
                    lstSaleOrg2DMS.Add(Const_OrgCode_Electric);
                    lstSaleOrg2DMS.Add(Const_OrgCode_Hubei);
                }

                return lstSaleOrg2DMS;
            }
        }

        // ������֯תDMS
        private static List<string> lstMfgOrg2DMS = new List<string>();
        /// <summary>
        /// ������֯תDMS
        /// </summary>
        public static List<string> MfgOrg2DMS
        {
            get
            {
                if (lstMfgOrg2DMS.Count == 0)
                {
                    lstMfgOrg2DMS.Add(Const_OrgCode_Electric);
                    lstMfgOrg2DMS.Add(Const_OrgCode_Chengdu);
                }

                return lstMfgOrg2DMS;
            }
        }

        // �ֽ�
        /// <summary>
        /// �ֽ�
        /// </summary>
        public const string Const_ShipDocType_XJ = "CK-XJ";
        // ����
        /// <summary>
        /// ����
        /// </summary>
        public const string Const_ShipDocType_SB = "CK-SB";
        // ������֯תDMS
        private static List<string> lstDMSShipDocType = new List<string>();
        /// <summary>
        /// ������֯תDMS
        /// </summary>
        public static List<string> DMSShipDocType
        {
            get
            {
                if (lstDMSShipDocType.Count == 0)
                {
                    lstDMSShipDocType.Add(Const_ShipDocType_XJ);
                    lstDMSShipDocType.Add(Const_ShipDocType_XJ);
                }

                return lstDMSShipDocType;
            }
        }

        public static bool IsOrg_Customer2DMS()
        {
            return PubHelper.SaleOrg2DMS.Contains(Context.LoginOrg.Code);
        }

        public static bool IsOrg_Supplier2DMS()
        {
            return PubHelper.MfgOrg2DMS.Contains(Context.LoginOrg.Code);
        }

        public static bool IsOrg_SupplierItem2DMS()
        {
            return PubHelper.MfgOrg2DMS.Contains(Context.LoginOrg.Code);
        }

        public static bool IsOrg_SalePriceList2DMS()
        {
            return PubHelper.SaleOrg2DMS.Contains(Context.LoginOrg.Code);
        }

        public static bool IsOrg_Finance2DMS()
        {
            return PubHelper.SaleOrg2DMS.Contains(Context.LoginOrg.Code);
        }

        public static bool IsWarehouse2DMS(Warehouse wh)
        {
            if (wh != null)
            {
                if (wh.DescFlexField != null
                    && wh.DescFlexField.PrivateDescSeg3.GetBool()
                    )
                {
                    return true;
                }

                if (wh.Code.StartsWith("SHBJ"))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsUpdateDMS(LotInfo lotinfo,out Supplier supt)
        {
            supt = null;
            if (lotinfo != null
                && lotinfo.LotMaster_EntityID != null
                )
            {
                string suptCode = lotinfo.LotMaster_EntityID.DescFlexSegments.PrivateDescSeg1;
                if (suptCode.IsNotNullOrWhiteSpace())
                {
                    supt = Supplier.Finder.Find("Code=@Code"
                        , new OqlParam(suptCode)
                        );

                    if (PubHelper.IsUpdateDMS(supt))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsUpdateDMS(SupplierMISCInfo supplier)
        {
            if (supplier != null
                && supplier.Supplier != null
                )
            {
                return IsUpdateDMS(supplier.Supplier);
            }
            return false;
        }

        public static bool IsUpdateDMS(Supplier supplier)
        {
            if (supplier != null
                && supplier.Category != null
                )
            {
                return supplier.Category.Code == "001"
                    || (supplier.Category != null
                        && supplier.Category.DescFlexField != null
                        && supplier.Category.DescFlexField.PrivateDescSeg1.GetBool()
                        );
            }

            return false;
        }

        public static bool IsUpdateDMS_Electric(ShipDocType shipDocType)
        {
            if (shipDocType != null
                && PubHelper.DMSShipDocType.Contains(shipDocType.Code)
                )
            {
                return true;
            }

            return false;
        }

        public static SalePriceLine GetSalePriceList(SupplierItem supplierItem)
        {
            string opath = string.Format("SalePriceList.Org={0} and ItemInfo.ItemID={1} and Active=1 and '{2}' between FromDate and ToDate ", Context.LoginOrg.ID.ToString(), supplierItem.ItemInfo.ItemID.ID.ToString(), System.DateTime.Now.ToString());
            if (Context.LoginOrg.Code == Const_OrgCode_Electric)
            {
                opath += string.Format(" and SalePriceList.Code='{0}'", Const_ElectricPartPriceListCode);
            }
            else if (Context.LoginOrg.Code == Const_OrgCode_Hubei)
            {
                //opath += string.Format(" and SalePriceList.Code={0}", Const_SalePartPriceListCode);
            }
            return SalePriceLine.Finder.Find(opath);
        }

        public static SalePriceLine GetSalePriceList(SupplySource supplierItem)
        {
            string opath = string.Format("SalePriceList.Org={0} and ItemInfo.ItemID={1} and Active=1 and '{2}' between FromDate and ToDate ", Context.LoginOrg.ID.ToString(), supplierItem.ItemInfo.ItemID.ID.ToString(), System.DateTime.Now.ToString());
            if (Context.LoginOrg.Code == Const_OrgCode_Electric)
            {
                opath += string.Format(" and SalePriceList.Code='{0}'", Const_ElectricPartPriceListCode);
            }
            else if (Context.LoginOrg.Code == Const_OrgCode_Hubei)
            {
                //opath += string.Format(" and SalePriceList.Code={0}", Const_SalePartPriceListCode);
            }
            return SalePriceLine.Finder.Find(opath);
        }

        public static string GetAddress(string oldurl)
        {
            string str = HttpRuntime.AppDomainAppPath.ToString();
            str += "\\bin\\DMSAPIServiceConfig.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(str);
            XmlNodeList list = null;

            //list = doc.GetElementsByTagName("services");

            if (Context.LoginOrg.Code == Const_OrgCode_Electric)
            {
                list = doc.GetElementsByTagName("servicesElectric");
            }
            else if (Context.LoginOrg.Code == Const_OrgCode_Hubei
                || Context.LoginOrg.Code == Const_OrgCode_Chengdu
                )
            {
                list = doc.GetElementsByTagName("servicesHubei");
            }

            if (list != null)
            {
                string newurl = list[0].Attributes["url"].Value;
                //string strr = oldurl.Replace("http://", "");
                //int t = strr.IndexOf("/");
                //string h = strr.Substring(0, t);
                //return oldurl.Replace(h, newurl);

                int index = oldurl.LastIndexOf("/");
                string svName = oldurl.Substring(index);

                newurl += svName;

                return newurl;
            }
            else
            {
                throw new BusinessException("û������DMS��ַ!");
            }
        }

		public static bool IsUsedDMSAPI()
		{
			string returnvalue = string.Empty;
			GetProfileValueProxy bpObj = new GetProfileValueProxy();
			bpObj.ProfileCode = ("IsUsedDMSAPI");
			PVDTOData pVTDOData = bpObj.Do();
			if (pVTDOData != null)
			{
				returnvalue = pVTDOData.ProfileValue;
			}
			return !(returnvalue.ToLower() == "false");
		}


	}
}
