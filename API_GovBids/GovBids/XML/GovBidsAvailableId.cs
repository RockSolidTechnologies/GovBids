﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;
namespace GovBids.XML
{
}

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class AvailBidIdResponse {
    
    private AvailBidIdResponseAvailBidIds[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("AvailBidIds", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public AvailBidIdResponseAvailBidIds[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class AvailBidIdResponseAvailBidIds {
    
    private AvailBidIdResponseAvailBidIdsBid[] bidField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Bid", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public AvailBidIdResponseAvailBidIdsBid[] Bid {
        get {
            return this.bidField;
        }
        set {
            this.bidField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class AvailBidIdResponseAvailBidIdsBid {
    
    private string itemIDField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ItemID {
        get {
            return this.itemIDField;
        }
        set {
            this.itemIDField = value;
        }
    }
}