// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Models {
    using System.Collections.Generic;

    /// <summary>
    /// Job info model
    /// </summary>
    public class JobInfoModel {

        /// <summary>
        /// Job id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Job generation
        /// </summary>
        public string GenerationId { get; set; }

        /// <summary>
        /// Job configuration
        /// </summary>
        public WriterGroupJobModel JobConfiguration { get; set; }

        /// <summary>
        /// Demands
        /// </summary>
        public List<DemandModel> Demands { get; set; }

        /// <summary>
        /// Redundancy configuration
        /// </summary>
        public RedundancyConfigModel RedundancyConfig { get; set; }

        /// <summary>
        /// Job lifetime
        /// </summary>
        public JobLifetimeDataModel LifetimeData { get; set; }
    }
}