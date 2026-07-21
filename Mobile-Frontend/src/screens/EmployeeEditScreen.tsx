import React, { useEffect, useState } from 'react';
import { View, StyleSheet, SafeAreaView, ScrollView, Alert, KeyboardAvoidingView, Platform, Switch, Text } from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { RootStackParamList } from '../../App';
import EmployeeService from '../services/employeeService';
import AppleTextInput from '../components/AppleTextInput';
import AppleButton from '../components/AppleButton';

type EmployeeEditScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'EmployeeEdit'>;
type EmployeeEditScreenRouteProp = RouteProp<RootStackParamList, 'EmployeeEdit'>;

interface Props {
  navigation: EmployeeEditScreenNavigationProp;
  route: EmployeeEditScreenRouteProp;
}

const EmployeeEditScreen: React.FC<Props> = ({ navigation, route }) => {
  const isEditing = !!route.params.id;
  const id = route.params.id;

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [department, setDepartment] = useState('');
  const [role, setRole] = useState(1);
  const [isActive, setIsActive] = useState(true);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    navigation.setOptions({ title: isEditing ? 'Edit Employee' : 'New Employee' });

    if (isEditing && id) {
      fetchEmployee(id);
    }
  }, [isEditing, id]);

  const fetchEmployee = async (employeeId: string) => {
    try {
      const data = await EmployeeService.getById(employeeId);
      setFirstName(data.firstName);
      setLastName(data.lastName);
      setEmail(data.email);
      setDepartment(data.department || '');
      setRole(data.role);
      setIsActive(data.isActive);
    } catch (error) {
      Alert.alert('Error', 'Failed to fetch employee details');
      navigation.goBack();
    }
  };

  const handleSave = async () => {
    if (!firstName || !lastName || !email) {
      Alert.alert('Validation Error', 'First Name, Last Name, and Email are required.');
      return;
    }

    setLoading(true);
    try {
      if (isEditing && id) {
        await EmployeeService.update(id, {
          firstName,
          lastName,
          email,
          department,
          role,
          isActive,
        });
      } else {
        await EmployeeService.create({
          firstName,
          lastName,
          email,
          department,
          role,
        });
      }
      navigation.goBack();
    } catch (error) {
      Alert.alert('Error', `Failed to ${isEditing ? 'update' : 'create'} employee`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      <KeyboardAvoidingView 
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        style={{ flex: 1 }}
      >
        <ScrollView contentContainerStyle={styles.scrollContent} keyboardShouldPersistTaps="handled">
          <View style={styles.formGroup}>
            <AppleTextInput
              label="First Name"
              placeholder="John"
              value={firstName}
              onChangeText={setFirstName}
            />
            <AppleTextInput
              label="Last Name"
              placeholder="Doe"
              value={lastName}
              onChangeText={setLastName}
            />
            <AppleTextInput
              label="Email"
              placeholder="john.doe@example.com"
              value={email}
              onChangeText={setEmail}
              keyboardType="email-address"
              autoCapitalize="none"
            />
            <AppleTextInput
              label="Department (Optional)"
              placeholder="Engineering"
              value={department}
              onChangeText={setDepartment}
            />
            
            <View style={styles.switchRow}>
              <Text style={styles.switchLabel}>Is Admin</Text>
              <Switch
                value={role === 2}
                onValueChange={(val) => setRole(val ? 2 : 1)}
                trackColor={{ false: '#E5E5EA', true: '#34C759' }}
              />
            </View>

            {isEditing && (
              <View style={styles.switchRow}>
                <Text style={styles.switchLabel}>Active Account</Text>
                <Switch
                  value={isActive}
                  onValueChange={setIsActive}
                  trackColor={{ false: '#E5E5EA', true: '#34C759' }}
                />
              </View>
            )}
          </View>

          <AppleButton
            title={isEditing ? 'Save Changes' : 'Create Employee'}
            onPress={handleSave}
            isLoading={loading}
            style={styles.saveButton}
          />
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F2F2F7',
  },
  scrollContent: {
    padding: 16,
  },
  formGroup: {
    marginBottom: 24,
  },
  switchRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#FFFFFF',
    padding: 16,
    borderRadius: 12,
    marginTop: 10,
    borderWidth: 1,
    borderColor: '#E5E5EA',
  },
  switchLabel: {
    fontSize: 17,
    color: '#000000',
  },
  saveButton: {
    marginTop: 16,
  },
});

export default EmployeeEditScreen;
