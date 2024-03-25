/********************************************************************************
 * Copyright (c) 2024 BMW Group AG
 *
 * See the NOTICE file(s) distributed with this work for additional
 * information regarding copyright ownership.
 *
 * This program and the accompanying materials are made available under the
 * terms of the Apache License, Version 2.0 which is available at
 * https://www.apache.org/licenses/LICENSE-2.0.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * SPDX-License-Identifier: Apache-2.0
 ********************************************************************************/

using Dim.DbAccess;
using Dim.DbAccess.Repositories;
using Dim.Entities.Enums;
using Microsoft.Extensions.Options;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;
using System.Text.RegularExpressions;

namespace Dim.Web.BusinessLogic;

public class DimBusinessLogic : IDimBusinessLogic
{
    private readonly IDimRepositories _dimRepositories;
    private readonly DimSettings _settings;

    public DimBusinessLogic(IDimRepositories dimRepositories, IOptions<DimSettings> options)
    {
        _dimRepositories = dimRepositories;
        _settings = options.Value;
    }

    public async Task StartSetupDim(string companyName, string bpn, string didDocumentLocation, bool isIssuer)
    {
        var processStepRepository = _dimRepositories.GetInstance<IProcessStepRepository>();
        var processId = processStepRepository.CreateProcess(ProcessTypeId.SETUP_DIM).Id;
        processStepRepository.CreateProcessStep(ProcessStepTypeId.CREATE_SUBACCOUNT, ProcessStepStatusId.TODO, processId);

        _dimRepositories.GetInstance<ITenantRepository>().CreateTenant(companyName, bpn, didDocumentLocation, isIssuer, processId, _settings.OperatorId);

        await _dimRepositories.SaveAsync().ConfigureAwait(false);
    }
}
