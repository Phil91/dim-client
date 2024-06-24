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

using Dim.Clients.Api.Cf;
using Dim.DbAccess;
using Dim.DbAccess.Repositories;
using Dim.Entities.Entities;
using Dim.Entities.Enums;
using DimProcess.Library.Callback;
using Org.Eclipse.TractusX.Portal.Backend.Framework.ErrorHandling;

namespace DimProcess.Library.Tests;

public class TechnicalUserDeletionProcessHandlerTests
{
    private readonly ITenantRepository _tenantRepositories;
    private readonly ICfClient _cfClient;
    private readonly ICallbackService _callbackService;
    private readonly TechnicalUserDeletionProcessHandler _sut;

    public TechnicalUserDeletionProcessHandlerTests()
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization { ConfigureMembers = true });
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var repositories = A.Fake<IDimRepositories>();
        _tenantRepositories = A.Fake<ITenantRepository>();

        A.CallTo(() => repositories.GetInstance<ITenantRepository>()).Returns(_tenantRepositories);

        _cfClient = A.Fake<ICfClient>();
        _callbackService = A.Fake<ICallbackService>();

        _sut = new TechnicalUserDeletionProcessHandler(repositories, _cfClient, _callbackService);
    }

    #region DeleteServiceInstanceBindings

    [Fact]
    public async Task DeleteServiceInstanceBindings_WithValidData_ReturnsExpected()
    {
        // Arrange
        var technicalUserId = Guid.NewGuid();
        var serviceBindingId = Guid.NewGuid();
        var spaceId = Guid.NewGuid();
        A.CallTo(() => _tenantRepositories.GetSpaceIdAndTechnicalUserName(technicalUserId))
            .Returns(new ValueTuple<Guid?, string>(spaceId, "test"));
        A.CallTo(() => _cfClient.GetServiceBinding("test", spaceId, A<string>._, A<CancellationToken>._))
            .Returns(serviceBindingId);

        // Act
        var result = await _sut.DeleteServiceInstanceBindings("test", technicalUserId, CancellationToken.None);

        // Assert
        result.modified.Should().BeFalse();
        result.processMessage.Should().BeNull();
        result.stepStatusId.Should().Be(ProcessStepStatusId.DONE);
        result.nextStepTypeIds.Should().ContainSingle().Which.Should().Be(ProcessStepTypeId.SEND_TECHNICAL_USER_DELETION_CALLBACK);
        A.CallTo(() => _cfClient.DeleteServiceInstanceBindings(A<Guid>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _cfClient.DeleteServiceInstanceBindings(serviceBindingId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeleteServiceInstanceBindings_WithoutSpaceId_ThrowsConflictException()
    {
        // Arrange
        var technicalUserId = Guid.NewGuid();
        A.CallTo(() => _tenantRepositories.GetSpaceIdAndTechnicalUserName(technicalUserId))
            .Returns(new ValueTuple<Guid?, string>(null, "test"));
        async Task Act() => await _sut.DeleteServiceInstanceBindings("test", technicalUserId, CancellationToken.None);

        // Act
        var ex = await Assert.ThrowsAsync<ConflictException>(Act);

        // Assert
        ex.Message.Should().Be("SpaceId must not be null.");
        A.CallTo(() => _cfClient.DeleteServiceInstanceBindings(A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    #endregion

    #region SendCallback

    [Fact]
    public async Task SendCallback_WithValidData_ReturnsExpected()
    {
        // Arrange
        var technicalUserId = Guid.NewGuid();
        var externalId = Guid.NewGuid();
        var technicalUsers = new List<TechnicalUser>
        {
            new(technicalUserId, Guid.NewGuid(), Guid.NewGuid(), "sa-t", Guid.NewGuid())
        };
        A.CallTo(() => _tenantRepositories.GetExternalIdForTechnicalUser(technicalUserId))
            .Returns(externalId);
        A.CallTo(() => _tenantRepositories.RemoveTechnicalUser(A<Guid>._))
            .Invokes((Guid tuId) =>
            {
                var user = technicalUsers.Single(x => x.Id == tuId);
                technicalUsers.Remove(user);
            });

        // Act
        var result = await _sut.SendCallback(technicalUserId, CancellationToken.None);

        // Assert
        result.modified.Should().BeFalse();
        result.processMessage.Should().BeNull();
        result.stepStatusId.Should().Be(ProcessStepStatusId.DONE);
        result.nextStepTypeIds.Should().BeNull();
        technicalUsers.Should().BeEmpty();
        A.CallTo(() => _callbackService.SendTechnicalUserDeletionCallback(A<Guid>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _callbackService.SendTechnicalUserDeletionCallback(externalId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    #endregion
}
