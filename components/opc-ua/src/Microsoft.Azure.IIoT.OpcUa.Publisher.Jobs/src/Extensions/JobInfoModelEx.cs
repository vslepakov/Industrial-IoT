// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Models {
    using System.Linq;

    /// <summary>
    /// Job extensions
    /// </summary>
    public static class JobInfoModelEx {

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static JobInfoModel Clone(this JobInfoModel model) {
            if (model == null) {
                return null;
            }
            return new JobInfoModel {
                Id = model.Id,
                Name = model.Name,
                Demands = model.Demands?.Select(d => d.Clone()).ToList(),
                JobConfiguration = model.JobConfiguration,  // TODO Clone()
                GenerationId = model.GenerationId,
                LifetimeData = model.LifetimeData?.Clone(),
                RedundancyConfig = model.RedundancyConfig.Clone(),
            };
        }
    }
}