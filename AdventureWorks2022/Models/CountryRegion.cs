﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AdventureWorks2022.Models;

/// <summary>
/// Lookup table containing the ISO standard codes for countries and regions.
/// </summary>
public partial class CountryRegion
{
    /// <summary>
    /// ISO standard code for countries and regions.
    /// </summary>
    public string CountryRegionCode { get; set; }

    /// <summary>
    /// Country or region name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<CountryRegionCurrency> CountryRegionCurrency { get; set; } = new List<CountryRegionCurrency>();

    public virtual ICollection<SalesTerritory> SalesTerritory { get; set; } = new List<SalesTerritory>();

    public virtual ICollection<StateProvince> StateProvince { get; set; } = new List<StateProvince>();
}